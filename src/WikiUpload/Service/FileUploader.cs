using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WikiUpload.Properties;

namespace WikiUpload
{
    public sealed class FileUploader : IDisposable, IFileUploader
    {
        private ApiUri _api;
        private string _editToken;
        private HttpClient _client;
        private PermittedFiles _permittedFiles;
        private bool _useDeprecatedLogin;
        private CookieContainer _cookies;
        private readonly string _userAgent;
        private readonly int _timeoutSeconds;

        private readonly Regex _isFandomDomainMatch = new Regex(@"^https://.+?\.fandom.com/", RegexOptions.IgnoreCase);

        public string PageContent { get; set; }

        public string Summary { get; set; }

        public string Site { get; set; }

        public string HomePage { get; set; }

        public string ScriptPath { get; set; }

        public bool CanUploadVideos => _isFandomDomainMatch.IsMatch(Site);

        public IReadOnlyPermittedFiles PermittedFiles
            => (IReadOnlyPermittedFiles)_permittedFiles;

        public FileUploader(string userAgent, int timeoutSeconds = 0)
        {
            PageContent = "";
            Summary = "";
            _permittedFiles = new PermittedFiles();
            _timeoutSeconds = timeoutSeconds;
            _userAgent = userAgent;
            CreateClient();
        }

        private void CreateClient()
        {
            var handler = new HttpClientHandler();
            _cookies = new CookieContainer();
            handler.CookieContainer = _cookies;
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            if (_timeoutSeconds > 0)
                _client.Timeout = new TimeSpan(0, 0, _timeoutSeconds);
        }

        public void LogOff()
        {
            _client.Dispose();
            CreateClient();
        }

        private void LogOffIfCookiesPresent()
        {
            if (_cookies.Count > 0)
                LogOff();
        }

        public async Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false)
        {
            try
            {
                Site = site;
                _api = new ApiUri(Site);

                var loginParams = new RequestParameters
                {
                    { "action", "login" },
                    { "format", "xml" },
                    { "lgname", username },
                };

                string loginToken = await GetLoginTokenAsync().ConfigureAwait(false);
                _useDeprecatedLogin = (loginToken == null);
                if (_useDeprecatedLogin)
                {
                    // use old method to fetch login token by loging in without password, required for fandom
                    var tokenResponse = await AttemptLoginAsync(loginParams).ConfigureAwait(false);
                    if (tokenResponse.Result != ResponseCodes.NeedToken)
                        return false;
                    loginToken = tokenResponse.Token;
                }

                loginParams.Add("lgtoken", loginToken);

                var response = password.UseUnsecuredString<LoginResponse>(unsecuredPassword =>
                {
                    // I can;t help feeling that all this effort to keep the password out of
                    // managed memory is pointless, HttpClient probably needs toplace it in
                    // managed memory anyway when it builds the request  <sigh>
                    loginParams.Add("lgpassword", unsecuredPassword);
                    var result = AttemptLoginAsync(loginParams).GetAwaiter().GetResult();
                    loginParams.Clear();
                    loginParams = null;
                    return result;
                });

                if (response.Result == ResponseCodes.Aborted)
                    throw new LoginException(Resources.LoginExceptionAborted);

                if (response.Result != ResponseCodes.Success)
                    return false;

                Task<string> editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
                Task<bool> userConfirmedTask = IsUserConfirmedAsync(username);
                Task<bool> authorizedTask = IsAuthorizedForUploadFilesAsync(username);
                Task<SiteInfo> siteInfoTask = GeSiteInfoAsync();

                await Task.WhenAll(userConfirmedTask, authorizedTask, editTokenTask, siteInfoTask)
                    .ConfigureAwait(false);

                // From here on in if we fail the login we must also log out, as the
                // sucessfull login session cookies will cause any subsequent login
                // attempts to the same site to fail with an aborted response.

                if (!userConfirmedTask.Result)
                {
                    LogOff();
                    throw new LoginException(Resources.LoginExceptionNotAutoConfirmed);
                }

                if (!authorizedTask.Result)
                {
                    LogOff();
                    throw new LoginException(Resources.LoginExceptionNotAuthorized);
                }

                if (string.IsNullOrEmpty(editTokenTask.Result))
                {
                    LogOff();
                    throw new LoginException(Resources.LoginExceptionNoEditToken);
                }

                HomePage = siteInfoTask.Result.BaseUrl;
                ScriptPath = siteInfoTask.Result.ScriptPath;

                if (!allFilesPermitted)
                {
                    foreach (string ext in siteInfoTask.Result.Extensions)
                        _permittedFiles.Add(ext);
                }

                _editToken = editTokenTask.Result;
                return true;
            }
            catch (XmlException)
            {
                LogOffIfCookiesPresent();
                throw new LoginException(Resources.LoginExceptionInvalidResponse);
            }
            catch (HttpRequestException ex)
            {
                LogOffIfCookiesPresent();
                throw new LoginException(ex.Message, ex.InnerException);
            }
            catch (TaskCanceledException)
            {
                LogOffIfCookiesPresent();
                throw new LoginException(Resources.LoginExceptionTimeout);
            }
            catch (AggregateException ex)
            {
                LogOffIfCookiesPresent();
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is TaskCanceledException)
                        throw new LoginException(Resources.LoginExceptionTimeout);
                    else if (innerEx is HttpRequestException)
                        throw new LoginException(innerEx.Message, innerEx.InnerException);
                    else if (innerEx is XmlException)
                        throw new LoginException(Resources.LoginExceptionInvalidResponse);
                }
                throw new LoginException(Resources.LoginExceptionUnexpectedError);
            }
        }

        public async Task<IUploadResponse> UpLoadAsync(string fullPath, CancellationToken cancelToken, bool ignoreWarnings = false, bool includeInWatchlist = false)
        {
            string fileName = Path.GetFileName(fullPath);

            // Open file 1st before we allocate any disposable resources to avoid leaks if
            // the file has be come inaccesible and an exception is thrown
            FileStream file = File.OpenRead(fullPath);

            // we don't need to dispose any of these or close the stream
            // as _client.PostAsync will do it

            var uploadFormData = new MultipartFormDataContent
            {
                { new StringContent("upload"), "action" },
                { new StringContent("5"), "maxlag" },
                { new StringContent(includeInWatchlist ? "watch" : "nochange"), "watchlist" },
                { new StringContent(fileName), "filename" },
                { new StringContent("xml"), "format" },
                { new StringContent(PageContent), "text" },
                { new StringContent(Summary), "comment" },
            };

            if (ignoreWarnings)
                uploadFormData.Add(new StringContent("1"), "ignorewarnings");

            uploadFormData.Add(new StreamContent(file), "file", fileName);

            uploadFormData.Add(new StringContent(_editToken), "token");

            using (HttpResponseMessage response = await _client.PostAsync(_api, uploadFormData, cancelToken).ConfigureAwait(false))
            {
                string retryAfter = "";
                if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string> retryValues))
                    retryAfter = retryValues.ElementAt(0);
                string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new UploadResponse(responseContent, retryAfter);
            }
        }

        public async Task<IngestionControllerResponse> UpLoadVideoAsync(string fullPath, CancellationToken cancelToken)
        {
            var queryParams = new RequestParameters
            {
                {  "controller", @"Fandom\Video\IngestionController" },
                {  "method", "uploadVideo" },
            };
            var uploadVideoUri = new Uri(Site + "/wikia.php" + queryParams.ToString());
            var formParams = new RequestParameters
            {
                { "url", fullPath },
                { "token", _editToken },
            };
            var req = new HttpRequestMessage(HttpMethod.Post, uploadVideoUri);
            req.Content = new FormUrlEncodedContent(formParams);

            IngestionControllerResponse videoUploadResponse;
            using (HttpResponseMessage response = await _client.SendAsync(req, cancelToken).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    videoUploadResponse = JsonConvert.DeserializeObject<IngestionControllerResponse>(responseContent);
                }
                else
                {
                    videoUploadResponse = new IngestionControllerResponse
                    {
                        Status = VideoUploadResponseMessage(response.StatusCode, response.ReasonPhrase),
                        Success = false
                    };
                }
                videoUploadResponse.HttpStatusCode = response.StatusCode;
            }

            return videoUploadResponse;
        }

        private string VideoUploadResponseMessage(HttpStatusCode statusCode, string reason)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return $"[{(int)statusCode} {reason}] {Resources.NoVideoUploadSupport} ";

                case HttpStatusCode.BadRequest:
                    return $"[{(int)statusCode} {reason}] {Resources.BadVideoUploadRequest}";

                default:
                    return $"[{(int)statusCode}] {reason}]";
            }
        }


        public async Task RefreshTokenAsync()
        {
            Task<string> editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
            _editToken = await editTokenTask.ConfigureAwait(false);
        }

        private async Task<bool> IsUserConfirmedAsync(string username)
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "users" },
                { "usprop", "groups" },
                { "ususers", username },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/users/user/groups/g[.=\"autoconfirmed\"]").ConfigureAwait(false);
            return node != null;
        }

        private async Task<string> GetEditTokenViaIntokenAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "prop", "info" },
                { "intoken", "edit" },
                { "titles", "6EA4096B-EBD2-4B9D-9025-2BA38D336E43" },
                { "indexpageids", "1" },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/pages/page").ConfigureAwait(false);
            return node?.Attributes["edittoken"]?.Value;
        }

        private async Task<string> GetEditTokenAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "tokens" },
                { "type", "csrf" },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/tokens").ConfigureAwait(false);
            return node?.Attributes["csrftoken"]?.Value;
        }

        private async Task<string> GetLoginTokenAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "tokens" },
                { "type", "login" },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/tokens").ConfigureAwait(false);
            return node?.Attributes["logintoken"]?.Value;
        }

        private async Task<SiteInfo> GeSiteInfoAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "siteinfo" },
                { "siprop", "general|fileextensions" },
            });
            XmlDocument doc = await GetXmlResponse(uri).ConfigureAwait(false);
            return new SiteInfo(doc);
        }

        private async Task<bool> IsAuthorizedForUploadFilesAsync(string username)
        {
            const char comment = '$';
            Regex tagRegex = new Regex(@"<.*>");

            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "prop", "revisions" },
                { "titles", "MediaWiki:Custom-WikiUpUsers" },
                { "rvprop", "content" },
                { "rvlimit", "1" },
            });
            XmlNode revision = await GetSingleNode(uri, "/api/query/pages/page/revisions/rev").ConfigureAwait(false);

            return revision == null ||
                revision.InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => x[0] != comment && !tagRegex.IsMatch(x))
                .Contains(username.ToUpperInvariant());
        }

        private async Task<LoginResponse> AttemptLoginAsync(List<KeyValuePair<string, string>> loginParams)
        {
            using (var formParams = new FormUrlEncodedContent(loginParams))
            {
                using (HttpResponseMessage response = await _client.PostAsync(_api, formParams).ConfigureAwait(false))
                {
                    XmlDocument xml = await GetXml(response.Content).ConfigureAwait(false);
                    XmlNode login = xml.SelectSingleNode("/api/login");
                    return new LoginResponse
                    {
                        Result = login?.Attributes["result"]?.Value,
                        Token = login?.Attributes["token"]?.Value,
                    };
                }
            }
        }
        public async Task<SearchResponse> FetchCategories(string from)
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "allcategories" },
                { "acfrom", from },
                { "aclimit", "50" },
                { "rawcontinue", "" },
            });
            XmlDocument doc = await GetXmlResponse(uri).ConfigureAwait(false);
            return SearchResponse.FromCategoryXml(doc);
        }

        public async Task<SearchResponse> FetchTemplates(string from)
        {
            const string templateNamespace = "10";
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "allpages" },
                { "apnamespace", templateNamespace },
                { "apfrom", from },
                { "aplimit", "100" },
                { "rawcontinue", "" },
            });
            XmlDocument doc = await GetXmlResponse(uri).ConfigureAwait(false);
            return SearchResponse.FromTemplateXml(doc);
        }

        private async Task<XmlNodeList> GetNodes(Uri uri, string path)
        {
            XmlDocument xml = await GetXmlResponse(uri).ConfigureAwait(false);
            return xml.SelectNodes(path);
        }

        private async Task<XmlNode> GetSingleNode(Uri uri, string path)
        {
            XmlDocument xml = await GetXmlResponse(uri).ConfigureAwait(false);
            return xml.SelectSingleNode(path);
        }

        private async Task<XmlDocument> GetXmlResponse(Uri uri)
        {
            string response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);
            return xml;
        }

        private static async Task<XmlDocument> GetXml(HttpContent content)
        {
            string response = await content.ReadAsStringAsync().ConfigureAwait(false);
            var doc = new XmlDocument();
            doc.LoadXml(response);
            return doc;
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }

    }
}