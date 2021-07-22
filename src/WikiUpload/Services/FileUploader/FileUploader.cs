using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal sealed class FileUploader : IDisposable, IFileUploader
    {
        private ApiUri _api;
        private string _editToken;
        private HttpClient _client;
        private readonly PermittedFiles _permittedFiles;
        private bool _useDeprecatedLogin;
        private CookieContainer _cookies;
        private readonly string _userAgent;
        private readonly int _timeoutSeconds;
        private string _errorLanguageCode;
        private bool _useErrorLang;
        private ISiteInfo _siteInfo;
        private readonly Regex _isFandomDomainMatch = new Regex(@"^https://.+?\.fandom.com/", RegexOptions.IgnoreCase);

        public string Site { get; private set; }

        public bool IncludeInWatchList { get; set; }

        public bool IgnoreWarnings { get; set; }

        public bool CanUploadVideos => _isFandomDomainMatch.IsMatch(Site);

        public ISiteInfo SiteInfo => _siteInfo;

        public IReadOnlyPermittedFiles PermittedFiles
            => _permittedFiles;

        public FileUploader(string userAgent, int timeoutSeconds = 0)
        {
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
                _client.Timeout = TimeSpan.FromSeconds( _timeoutSeconds);
        }

        public void LogOff()
        {
            _client.Dispose();
            _permittedFiles.Clear();
            CreateClient();
        }

        private void LogOffIfCookiesPresent()
        {
            if (_cookies.Count > 0)
                LogOff();
        }

        public async Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted)
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

                var loginToken = await GetLoginTokenAsync().ConfigureAwait(false);
                _useDeprecatedLogin = (loginToken == null);
                if (_useDeprecatedLogin)
                {
                    // use old method to fetch login token by loging in without password
                    var tokenResponse = await AttemptLoginAsync(loginParams).ConfigureAwait(false);
                    if (tokenResponse.Result != ResponseCodes.NeedToken)
                        return false;
                    loginToken = tokenResponse.Token;
                }

                loginParams.Add("lgtoken", loginToken);

                var response = await password.UseUnsecuredStringAsync<LoginResponse>((unsecuredPassword) =>
                {
                    loginParams.Add("lgpassword", unsecuredPassword);
                    return AttemptLoginAsync(loginParams);
                });

                if (response.Result == ResponseCodes.Aborted)
                    throw new LoginException(Resources.LoginExceptionAborted);

                if (response.Result != ResponseCodes.Success)
                    return false;

                var editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
                var userConfirmedTask = IsUserConfirmedAsync(username);
                var authorizedTask = IsAuthorizedForUploadFilesAsync(username);
                var siteInfoTask = GeSiteInfoAsync();

                await Task.WhenAll(userConfirmedTask, authorizedTask, editTokenTask, siteInfoTask)
                    .ConfigureAwait(false);

                // From here on in if we fail the login we must also log out, as the
                // sucessfull login session cookies will cause any subsequent login
                // attempts to the same site to fail with an aborted response.

                try
                {
                    if (!userConfirmedTask.Result)
                        throw new LoginException(Resources.LoginExceptionNotAutoConfirmed);

                    if (!authorizedTask.Result)
                        throw new LoginException(Resources.LoginExceptionNotAuthorized);

                    if (string.IsNullOrEmpty(editTokenTask.Result))
                        throw new LoginException(Resources.LoginExceptionNoEditToken);
                }
                catch (LoginException)
                {
                    LogOff();
                    throw;
                }

                _editToken = editTokenTask.Result;

                var siteInfo = siteInfoTask.Result;
                //HomePage = siteInfo.BaseUrl;
                //ScriptPath = siteInfo.ScriptPath;
                if (!allFilesPermitted)
                {
                    foreach (var ext in siteInfo.Extensions)
                        _permittedFiles.Add(ext);
                }

                var languageCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
                _errorLanguageCode = siteInfo.IsSupportedLanguage(languageCode) ? languageCode : "en";
                var useErrorLangVersion = new Version("1.29.0.0");
                _useErrorLang = siteInfo.MediaWikiVersion >= useErrorLangVersion;
                _siteInfo = siteInfo;

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

        public async Task<IUploadResponse> UpLoadAsync(string fullPath,
            CancellationToken cancelToken,
            string summary,
            string newPageContent)
        {
            var fileName = Path.GetFileName(fullPath);

            // Open file 1st before we allocate any disposable resources to avoid leaks if
            // the file has become inaccesible and an exception is thrown
            var file = File.OpenRead(fullPath);

            // we don't need to dispose any of these or close the stream
            // as _client.PostAsync will do it

            var uploadFormData = new MultipartFormDataContent
            {
                { new StringContent("upload"), "action" },
                { new StringContent("5"), "maxlag" },
                { new StringContent(IncludeInWatchList ? "watch" : "nochange"), "watchlist" },
                { new StringContent(fileName), "filename" },
                { new StringContent("xml"), "format" },
                { new StringContent(newPageContent), "text" },
                { new StringContent(summary), "comment" },
            };

            if (_useErrorLang)
            {
                uploadFormData.Add(new StringContent("plaintext"), "errorformat");
                uploadFormData.Add(new StringContent(_errorLanguageCode), "errorlang");
            }

            if (IgnoreWarnings)
                uploadFormData.Add(new StringContent("1"), "ignorewarnings");

            uploadFormData.Add(new StreamContent(file), "file", fileName);

            uploadFormData.Add(new StringContent(_editToken), "token");

            using (var response = await _client.PostAsync(_api, uploadFormData, cancelToken).ConfigureAwait(false))
            {
                var retryAfter = "";
                if (response.Headers.TryGetValues("Retry-After", out var retryValues))
                    retryAfter = retryValues.ElementAt(0);
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                return new UploadResponse(responseContent, retryAfter);
            }
        }

        public async Task<IngestionControllerResponse> UpLoadVideoAsync(string fullPath, CancellationToken cancelToken)
        {
            var formParams = new RequestParameters
            {
                { "url", fullPath },
                { "token", _editToken },
            };

            using (var request = new HttpRequestMessage(HttpMethod.Post, UploadVideoUri()))
            {
                request.Content = new FormUrlEncodedContent(formParams);
                using (var response = await _client.SendAsync(request, cancelToken).ConfigureAwait(false))
                    return await ProcessUploadVideoResponse(response);
            }
        }

        private async Task<IngestionControllerResponse> ProcessUploadVideoResponse(HttpResponseMessage response)
        {
            IngestionControllerResponse videoUploadResponse;

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
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
            return videoUploadResponse;
        }


        private Uri UploadVideoUri()
        {
            var queryParams = new RequestParameters
            {
                {  "controller", @"Fandom\Video\IngestionController" },
                {  "method", "uploadVideo" },
            };
            return new Uri(Site + "/wikia.php" + queryParams.ToString());
        }

        private static string VideoUploadResponseMessage(HttpStatusCode statusCode, string reason)
        {
            switch (statusCode)
            {
                case HttpStatusCode.NotFound:
                    return $"[{(int)statusCode} {reason}] {Resources.NoVideoUploadSupport} ";

                case HttpStatusCode.BadRequest:
                    return $"[{(int)statusCode} {reason}] {Resources.BadVideoUploadRequest}";

                default:
                    return $"[{(int)statusCode} {reason}]";
            }
        }


        public async Task RefreshTokenAsync()
        {
            var editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
            _editToken = await editTokenTask.ConfigureAwait(false);
        }

        private async Task<bool> IsUserConfirmedAsync(string username)
        {
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "users" },
                { "usprop", "groups" },
                { "ususers", username },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var node = GetSingleNode(response, "/api/query/users/user/groups/g[.=\"autoconfirmed\"]");
            return node != null;
        }

        private async Task<string> GetEditTokenViaIntokenAsync()
        {
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "prop", "info" },
                { "intoken", "edit" },
                { "titles", "6EA4096B-EBD2-4B9D-9025-2BA38D336E43" },
                { "indexpageids", "1" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var node = GetSingleNode(response, "/api/query/pages/page");
            return node?.Attributes["edittoken"]?.Value;
        }

        private async Task<string> GetEditTokenAsync()
        {
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "tokens" },
                { "type", "csrf" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var node = GetSingleNode(response, "/api/query/tokens");
            return node?.Attributes["csrftoken"]?.Value;
        }

        private async Task<string> GetLoginTokenAsync()
        {
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "tokens" },
                { "type", "login" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var node = GetSingleNode(response, "/api/query/tokens");
            return node?.Attributes["logintoken"]?.Value;
        }

        private async Task<SiteInfo> GeSiteInfoAsync()
        {
            var siteproperties = new string[]
            {
                "general",
                "fileextensions",
                "languages",
                "namespaces"
            };
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "siteinfo" },
                { "siprop", string.Join("|", siteproperties) },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var xml = CreateXmlDocument(response);
            return new SiteInfo(xml);
        }

        private async Task<bool> IsAuthorizedForUploadFilesAsync(string username)
        {
            const char comment = '$';
            var tagRegex = new Regex(@"<.*>");

            var uri = _api.ApiQuery(new RequestParameters
            {
                { "prop", "revisions" },
                { "titles", "MediaWiki:Custom-WikiUpUsers" },
                { "rvprop", "content" },
                { "rvlimit", "1" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var revision = GetSingleNode(response, "/api/query/pages/page/revisions/rev");

            return revision == null ||
                revision.InnerText.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim().ToUpperInvariant())
                .Where(x => x.Length > 0 && x[0] != comment && !tagRegex.IsMatch(x))
                .Contains(username.ToUpperInvariant());
        }

        private async Task<LoginResponse> AttemptLoginAsync(List<KeyValuePair<string, string>> loginParams)
        {
            using (var formParams = new FormUrlEncodedContent(loginParams))
            {
                using (var response = await _client.PostAsync(_api, formParams).ConfigureAwait(false))
                {
                    var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var login = GetSingleNode(responseContent, "/api/login");
                    var loginResponse = new LoginResponse();
                    if (login != null)
                    {
                        loginResponse.Result = login.Attributes["result"]?.Value;
                        loginResponse.Token = login.Attributes["token"]?.Value;
                    }
                    return loginResponse;
                }
            }
        }
        public async Task<SearchResponse> FetchCategories(string from)
        {
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "allcategories" },
                { "acfrom", from },
                { "aclimit", "50" },
                { "rawcontinue", "" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var xml = CreateXmlDocument(response);
            return SearchResponse.FromCategoryXml(xml);
        }

        public async Task<SearchResponse> FetchTemplates(string from)
        {
            const string templateNamespace = "10";
            var uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "allpages" },
                { "apnamespace", templateNamespace },
                { "apfrom", from },
                { "aplimit", "100" },
                { "rawcontinue", "" },
            });
            var response = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var xml = CreateXmlDocument(response);
            return SearchResponse.FromTemplateXml(xml);
        }

        private XmlNode GetSingleNode(string xmlString, string path)
            => CreateXmlDocument(xmlString).SelectSingleNode(path);

        private static XmlDocument CreateXmlDocument(string xmlString)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xmlString);
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

        public string FileUrl(string fileName)
        {
            var name = fileName.ToCharArray();
            for (int i = 0; i < name.Length; i++)
            {
                if (name[i] == ' ')
                    name[i] = '_';
            }
            if (SiteInfo.WikiCasing == WikiCasing.FirstLetter)
                name[0] = char.ToUpper(name[0]);
            return SiteInfo.ServerUrl 
                + SiteInfo.ArticlePath.Replace("$1", SiteInfo.FileNamespace + new string(name));
        }

        public string ServerFilename(string fileName)
            => SiteInfo.WikiCasing == WikiCasing.FirstLetter ? fileName.CapitalizeFirstLetter() : fileName;
    }
}