using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;

namespace TestServer
{
    internal class Program
    {
        private const string loginToken = "123456+\\";


        private static readonly object randLock = new();
        private static readonly Random rand = new();
        private static long videoUploadCount = 0;
        private static long fileUploadCount = 0;
        private static Options options;
        private static ILog log;

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Run(options));
        }

        private static void Run(Options o)
        {
            options = o;
            log = Logging.Create(options.NoLog);
            var listener = CreateListener();
            WriteStartupMessage(listener);
            listener.Start();
            while (true)
            {
                var context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(HandleRequest, context);
            }
        }

        private static void WriteStartupMessage(HttpListener listener)
        {
            Console.WriteLine($"Listening on");
            foreach (var prefix in listener.Prefixes)
                Console.WriteLine($"  {prefix}");
        }

        private static HttpListener CreateListener()
        {
            var listener = new HttpListener();

            foreach (var prefix in options.Prefixes)
                listener.Prefixes.Add("http://" + prefix);

            if (listener.Prefixes.Count == 0)
            {
                listener.Prefixes.Add($"http://localhost:{options.Port}/");
                listener.Prefixes.Add($"http://localhost:{options.Port}/wiki/");
            }

            return listener;
        }

        private static int GetRandom(int max)
        {
            lock (randLock) return rand.Next(max);
        }

        private static void HandleRequest(object context)
        {
            try
            {
                DoHandleRequest((HttpListenerContext)context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DoHandleRequest(HttpListenerContext context)
        {
            var request = context.Request;
            ServerResponse serverResponse;

            log.Log($"[{Thread.CurrentThread.ManagedThreadId}] {request.RawUrl}");

            if (request.HasEntityBody)
                serverResponse = RequestBodyReply(request);
            else
                serverResponse = NoRequestBodyReply(request);

            if (!serverResponse.TimeoutRequest)
                serverResponse.Send(context.Response);

            if (options.ShowReply)
                log.Log($"{request.RawUrl}\nResponse:\n{serverResponse.Reply}\n");
        }


        private static ServerResponse RequestBodyReply(HttpListenerRequest request)
        {
            ServerResponse serverResponse;

            try
            {
                if (request.ContentType == "application/x-www-form-urlencoded")
                {
                    if (request.RawUrl.Contains("ingestion", StringComparison.InvariantCultureIgnoreCase))
                        serverResponse = VideoUploadReply();
                    else
                        serverResponse = LoginReply(request);
                }   
                else if (request.ContentType.StartsWith("multipart/form-data"))
                {
                    serverResponse = UploadReply();
                }
                else
                {
                    Console.WriteLine("ERROR: Unknown content type");
                    serverResponse = new ServerResponse { Reply = ApiReply("") };
                }
            }
            finally
            {
                request.InputStream.Close();
            }

            return serverResponse;
        }

        private static ServerResponse VideoUploadReply()
        {
            if (options.Delay > 0)
                Thread.Sleep(options.Delay);

            var serverResponse = new ServerResponse();
            var count = options.VideoErrors ? Interlocked.Increment(ref videoUploadCount) % 5 : 0;
            switch (count)
            {
                case 0:
                    serverResponse.Reply = "{\"success\": true, \"status\": \"\"}";
                    break;
                case 1:
                    serverResponse.Reply = "{\"success\": false, \"status\": \"Video already exists\"}";
                    break;
                case 2:
                    serverResponse.StatusCode = 400;
                    break;
                case 3:
                    serverResponse.StatusCode = 404;
                    break;
                case 4:
                    serverResponse.StatusCode = 500;
                    break;
            }
            return serverResponse;
        }

        private static ServerResponse NoRequestBodyReply(HttpListenerRequest request)
        {
            var serverResponse = new ServerResponse();

            if (request.RawUrl.Contains("list=users&usprop=groups&ususers"))
                serverResponse.Reply = QueryReply(Replies.UserGroups);

            else if (request.RawUrl.Contains("6EA4096B-EBD2-4B9D-9025-2BA38D336E43"))
                serverResponse.Reply = QueryReply(Replies.EditTokenPage);

            else if (WebUtility.UrlDecode(request.RawUrl).Contains("MediaWiki:Custom-WikiUpUsers"))
                serverResponse.Reply = QueryReply(Replies.AuthorizedUsers);

            else if (request.RawUrl.Contains("meta=siteinfo"))
                serverResponse.Reply = options.NoPermittedFiles
                    ? ApiReply("")
                    : QueryReply(Replies.SiteInfo);

            else if (request.RawUrl.ContainsAll(new List<string> { "meta=tokens", "type=login" }))
                serverResponse.Reply = options.OldLogin
                    ? QueryReply(Replies.NoMetaTokenSupport)
                    : QueryReply(string.Format(Replies.LoginToken, loginToken));

            else if (request.RawUrl.Contains("meta=tokens&type=csrf"))
                serverResponse.Reply = QueryReply(Replies.EditToken);

            else if (request.RawUrl.Contains("allcategories"))
                serverResponse.TimeoutRequest = true;

            else
                serverResponse.Reply = ApiReply("");

            return serverResponse;
        }

        private static ServerResponse UploadReply()
        {
            if (options.Delay > 0)
                Thread.Sleep(options.Delay);

            var serverResponse = new ServerResponse();

            var count = Interlocked.Increment(ref fileUploadCount);

            if (options.UploadTimeout > 0 && GetRandom(100) < options.UploadTimeout)
            {
                serverResponse.TimeoutRequest = true;
            }
            else if (options.BadTokens > 0 && count % options.BadTokens == 0)
            {
                serverResponse.Reply = ApiReply(Replies.BadToken);
            }
            else if (options.MustBeLoggedIn > 0 && count % options.MustBeLoggedIn == 0)
            {
                serverResponse.Reply = ApiReply(Replies.MustBeLoggedIn);
            }
            else if (options.MaxLag == -1 || (options.MaxLag > 0 && GetRandom(100) < options.MaxLag))
            {
                serverResponse.Reply = Replies.MaxLag;
                serverResponse.Headers.Add(new ServerResponseHeader("Retry-After", "5"));
            }
            else if (options.Exists > 0 && GetRandom(100) < options.Exists)
            {
                serverResponse.Reply = ApiReply(Replies.AlreadyExists);
            }
            else if (options.InvalidXml > 0 && GetRandom(100) < options.InvalidXml)
            {
                serverResponse.Reply = "Hey this is not xml";
            }
            else if (options.LongError > 0 && GetRandom(100) < options.LongError)
            {
                serverResponse.Reply = ApiReply(Replies.LongErrorMessasge);
            }
            else
            {
                serverResponse.Reply = ApiReply(Replies.UploadSuccess);
            }

            return serverResponse;
        }

        private static ServerResponse LoginReply(HttpListenerRequest request)
        {
            string reply;
            
            if (options.LoginTimeout)
                return new ServerResponse { TimeoutRequest = true };
            else if (options.InvalidLoginXml)
                reply = "Howdy = I'm not xml";
            else
            {
                string content;
                using (var sr = new StreamReader(request.InputStream, request.ContentEncoding))
                    content = sr.ReadToEnd();
                if (content.Contains("lgpassword="))
                {
                    CheckLoginToken(loginToken, content);
                    var password = Regex.Match(content, @"lgpassword=([^&$]*)").Groups[1].Value;
                    if (password == "" | password == "a")
                        reply = ApiReply(Replies.LpginSuccess);
                    else
                        reply = ApiReply(Replies.LoginFail);
                }
                else
                {
                    reply = ApiReply(string.Format(Replies.LoginNeedToken, loginToken));
                }
            }

            return new ServerResponse { Reply = reply };
        }

        private static void CheckLoginToken(string loginToken, string content)
        {
            var tokenMatch = Regex.Match(content, "lgtoken=([^&$]*)");
            if (tokenMatch.Success)
            {
                if (WebUtility.UrlDecode(tokenMatch.Groups[1].Value) != loginToken)
                    Console.WriteLine($"ERROR: Wrong login token = {tokenMatch.Groups[1].Value}");
            }
            else
            {
                Console.WriteLine("ERROR: No login token supplied");
            }
        }

        private static string ApiReply(string content)
            => $"<?xml version=\"1.0\"?><api>{content}</api>";

        private static string QueryReply(string content)
            => ApiReply($"<query>{content}</query>");
    }
}
