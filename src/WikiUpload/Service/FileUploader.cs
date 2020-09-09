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

namespace WikiUpload
{
    public sealed class FileUploader : IDisposable
    {
        private ApiUri _api;
        private string _editToken;
        private HttpClient _client;
        private PermittedFiles _permittedFiles;
        private bool _useDeprecatedLogin;

        public string PageContent { get; set; }

        public string Summary { get; set; }

        public string Site { get; set; }

        public IReadOnlyPermittedFiles PermittedFiles
            => (IReadOnlyPermittedFiles)_permittedFiles;

        public FileUploader(string userAgent, int timeoutSeconds = 0)
        {
            PageContent = "";
            Summary = "";
            _permittedFiles = new PermittedFiles();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = new CookieContainer();
            _client = new HttpClient(handler);
            _client.DefaultRequestHeaders.Add("User-Agent", userAgent);
            if (timeoutSeconds > 0)
                _client.Timeout = new TimeSpan(0, 0, timeoutSeconds);
        }

        public async Task<bool> LoginAsync(string site, string username, SecureString password, bool allFilesPermitted = false)
        {
            const string xmlExceptionmessage = "The server returned an invalid response.";
            const string timeoutExceptionMessage = "The server did not respond.";

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
                    throw new LoginException("You must use a bot password (username@password).");

                if (response.Result != ResponseCodes.Success)
                    return false;

                Task<string> editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
                Task<bool> userConfirmedTask = IsUserConfirmedAsync(username);
                Task<bool> authorizedTask = IsAuthorizedForUploadFilesAsync(username);

                if (allFilesPermitted)
                {
                    await Task.WhenAll(userConfirmedTask, authorizedTask, editTokenTask)
                        .ConfigureAwait(false);
                }
                else
                {
                    await Task.WhenAll(userConfirmedTask, authorizedTask, GetPermittedTypes(), editTokenTask)
                        .ConfigureAwait(false);
                }

                if (!userConfirmedTask.Result)
                    throw new LoginException("That account is not autoconfirmed.");

                if (!authorizedTask.Result)
                    throw new LoginException("You are not authorized to use Wiki-Up on this wiki.");

                if (string.IsNullOrEmpty(editTokenTask.Result))
                    throw new LoginException("Unable to obtain edit token.");

                _editToken = editTokenTask.Result;
                return true;
            }
            catch (XmlException)
            {
                throw new LoginException(xmlExceptionmessage);
            }
            catch (HttpRequestException ex)
            {
                throw new LoginException(ex.Message, ex.InnerException);
            }
            catch (TaskCanceledException)
            {
                throw new LoginException(timeoutExceptionMessage);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is TaskCanceledException)
                        throw new LoginException(timeoutExceptionMessage);
                    else if (innerEx is HttpRequestException)
                        throw new LoginException(innerEx.Message, innerEx.InnerException);
                    else if (innerEx is XmlException)
                        throw new LoginException(xmlExceptionmessage);
                }
                throw new LoginException("An unexpected error occured.");
            }
        }

        public async Task<UploadResponse> UpLoadAsync(string fullPath, CancellationToken cancelToken, bool ignoreWarnings = false)
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
                { new StringContent(fileName), "filename" },
                { new StringContent(_editToken), "token" },
                { new StringContent("xml"), "format" },
                { new StringContent(PageContent), "text" },
                { new StringContent(Summary), "comment" },
            };

            if (ignoreWarnings)
                uploadFormData.Add(new StringContent("1"), "ignorewarnings");

            uploadFormData.Add(new StreamContent(file), "file", fileName);

            using (HttpResponseMessage response = await _client.PostAsync(_api, uploadFormData, cancelToken))
            {
                string retryAfter = "";
                if (response.Headers.TryGetValues("Retry-After", out IEnumerable<string> retryValues))
                    retryAfter = retryValues.ElementAt(0);
                string responseContent = await response.Content.ReadAsStringAsync();
                return new UploadResponse(responseContent, retryAfter);
            }
        }

        public async Task RefreshTokenAsync()
        {
            Task<string> editTokenTask = _useDeprecatedLogin ? GetEditTokenViaIntokenAsync() : GetEditTokenAsync();
            _editToken = await editTokenTask;
        }

        private async Task<bool> IsUserConfirmedAsync(string username)
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "list", "users" },
                { "usprop", "groups" },
                { "ususers", username },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/users/user/groups/g[.=\"autoconfirmed\"]");
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
            XmlNode node = await GetSingleNode(uri, "/api/query/pages/page");
            return node?.Attributes["edittoken"]?.Value;
        }

        private async Task<string> GetEditTokenAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta","tokens" },
                { "type","csrf" },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/tokens");
            return node?.Attributes["csrftoken"]?.Value;
        }

        private async Task<string> GetLoginTokenAsync()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "tokens" },
                { "type", "login" },
            });
            XmlNode node = await GetSingleNode(uri, "/api/query/tokens");
            return node?.Attributes["logintoken"]?.Value;
        }

        private async Task GetPermittedTypes()
        {
            Uri uri = _api.ApiQuery(new RequestParameters
            {
                { "meta", "siteinfo" },
                { "siprop", "fileextensions" },
            });
            XmlNodeList fileExtensions = await GetNodes(uri, "/api/query/fileextensions/fe");
            foreach (XmlNode fe in fileExtensions)
                _permittedFiles.Add(fe.Attributes["ext"].Value);
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
            XmlNode revision = await GetSingleNode(uri, "/api/query/pages/page/revisions/rev");

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
                using (HttpResponseMessage response = await _client.PostAsync(_api, formParams))
                {
                    XmlDocument xml = await GetXml(response.Content);
                    XmlNode login = xml.SelectSingleNode("/api/login");
                    return new LoginResponse
                    {
                        Result = login?.Attributes["result"]?.Value,
                        Token = login?.Attributes["token"]?.Value,
                    };
                }
            }
        }

        private async Task<XmlNodeList> GetNodes(Uri uri, string path)
        {
            XmlDocument xml = await GetXmlResponse(uri);
            return xml.SelectNodes(path);
        }

        private async Task<XmlNode> GetSingleNode(Uri uri, string path)
        {
            XmlDocument xml = await GetXmlResponse(uri);
            return xml.SelectSingleNode(path);
        }

        private async Task<XmlDocument> GetXmlResponse(Uri uri)
        {
            string response = await _client.GetStringAsync(uri);
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(response);
            return xml;
        }

        private static async Task<XmlDocument> GetXml(HttpContent content)
        {
            string response = await content.ReadAsStringAsync();
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