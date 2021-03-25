using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace TestServer
{
    internal class Program
    {
        private const string loginToken = "123456+\\";


        private static readonly object randLock = new object();
        private static readonly Random rand = new Random();
        private static long videoUploadCount = 0;
        private static long fileUploadCount = 0;


        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(options => Run(options));
        }

        private static void Run(Options options)
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:10202/");
            listener.Prefixes.Add("http://localhost:10202/wiki/");
            Console.WriteLine("Listening...");
            listener.Start();
            while (true)
            {
                var context = listener.GetContext();
                ThreadPool.QueueUserWorkItem(x => HandleRequest(context, options));
            }
        }

        private static int GetRandom(int max)
        {
            lock (randLock) return rand.Next(max);
        }

        private static void HandleRequest(HttpListenerContext context, Options options)
        {
            try
            {
                DoHandleRequest(context, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void DoHandleRequest(HttpListenerContext context, Options options)
        {
            string reply; ;
            var request = context.Request;
            var response = context.Response;
            var statusCode = 200;

            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] {context.Request.RawUrl}");
            if (request.HasEntityBody)
            {
                if (request.ContentType == "application/x-www-form-urlencoded")
                {
                    if (request.RawUrl.IndexOf("ingestion", StringComparison.InvariantCultureIgnoreCase) == -1)
                    {
                        if (options.LoginTimeout)
                            return;
                        reply = LoginReply(options, request);
                    }
                    else
                    {
                        reply = "";
                        VideoUploadReply(ref reply, ref statusCode, options);
                    }
                }
                else if (request.ContentType.StartsWith("multipart/form-data"))
                {
                    if (options.UploadTimeout > 0 && GetRandom(100) < options.UploadTimeout)
                        return;
                    reply = UploadReply(options, response);
                }
                else
                {
                    Console.WriteLine("ERROR: Unknown content type");
                    reply = ApiReply("");
                }
                request.InputStream.Close();
            }
            else
            {
                reply = NoRequestBodyReply(options, request);
                if (reply == null)
                    return;
            }

            response.StatusCode = statusCode;
            var buffer = Encoding.UTF8.GetBytes(reply);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            if (options.ShowReply)
                Console.WriteLine($"{request.RawUrl}\nResponse:\n{reply}\n");
        }

        private static void VideoUploadReply(ref string reply, ref int statusCode, Options options)
        {
            var count = options.VideoErrors ? Interlocked.Increment(ref videoUploadCount) % 5 : 0;
            switch (count)
            {
                case 0:
                    reply = "{\"success\": true, \"status\": \"\"}";
                    break;
                case 1:
                    reply = "{\"success\": false, \"status\": \"Video already exists\"}";
                    break;
                case 2:
                    statusCode = 400;
                    break;
                case 3:
                    statusCode = 404;
                    break;
                case 4:
                    statusCode = 500;
                    break;
            }
        }

        private static string NoRequestBodyReply(Options options, HttpListenerRequest request)
        {
            string reply;

            if (request.RawUrl.IndexOf("list=users&usprop=groups&ususers") != -1)
                reply = QueryReply(Replies.UserGroups);

            else if (request.RawUrl.IndexOf("6EA4096B-EBD2-4B9D-9025-2BA38D336E43") != -1)
                reply = QueryReply(Replies.EditTokenPage);

            else if (HttpUtility.UrlDecode(request.RawUrl).IndexOf("MediaWiki:Custom-WikiUpUsers") != -1)
                reply = QueryReply(Replies.AuthorizedUsers);

            else if (request.RawUrl.IndexOf("meta=siteinfo") != -1)
                reply = options.NoPermittedFiles
                    ? ApiReply("")
                    : QueryReply(Replies.SiteInfo);

            else if (request.RawUrl.ContainsAll(new List<string> { "meta=tokens", "type=login" }))
                reply = options.OldLogin
                    ? QueryReply(Replies.NoMetaTokenSupport)
                    : QueryReply(string.Format(Replies.LoginToken, loginToken));

            else if (request.RawUrl.IndexOf("meta=tokens&type=csrf") != -1)
                reply = QueryReply(Replies.EditToken);

            else if (request.RawUrl.IndexOf("allcategories") != -1)
                reply = null;

            else
                reply = ApiReply("");

            return reply;
        }

        private static string UploadReply(Options options, HttpListenerResponse response)
        {
            string reply;

            var count = Interlocked.Increment(ref fileUploadCount);
            if (options.BadTokens > 0 && count % options.BadTokens == 0)
                reply = ApiReply(Replies.BadToken);
            else if (options.MaxLag == -1)
                reply = MaxLagReply(response);
            else if (options.Exists > 0 && GetRandom(100) < options.Exists)
                reply = ApiReply(Replies.AlreadyExists);
            else if (options.InvalidXml > 0 && GetRandom(100) < options.InvalidXml)
                reply = "Hey this is not xml";
            else if (options.MaxLag > 0 && GetRandom(100) < options.MaxLag)
                reply = MaxLagReply(response);
            else if (options.LongError > 0 && GetRandom(100) < options.LongError)
                reply = ApiReply(Replies.LongErrorMessasge);
            else
                reply = ApiReply(Replies.UploadSuccess);

            if (options.Delay > 0)
                Thread.Sleep(options.Delay);
            return reply;
        }

        private static string LoginReply(Options options, HttpListenerRequest request)
        {
            string reply;
            if (options.InvalidLoginXml)
                reply = "Howdy = I'm not xml";
            else
            {
                string content;
                using (var sr = new StreamReader(request.InputStream, request.ContentEncoding))
                    content = sr.ReadToEnd();
                if (content.IndexOf("lgpassword=") != -1)
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

            return reply;
        }

        private static void CheckLoginToken(string loginToken, string content)
        {
            var tokenMatch = Regex.Match(content, "lgtoken=([^&$]*)");
            if (tokenMatch.Success)
            {
                if (HttpUtility.UrlDecode(tokenMatch.Groups[1].Value) != loginToken)
                    Console.WriteLine($"ERROR: Wrong login token = {tokenMatch.Groups[1].Value}");
            }
            else
            {
                Console.WriteLine("ERROR: No login token supplied");
            }
        }

        private static string MaxLagReply(HttpListenerResponse response)
        {
            response.AddHeader("Retry-After", "5");
            return ApiReply(Replies.MaxLag);
        }

        private static string ApiReply(string content)
            => $"<?xml version=\"1.0\"?><api>{content}</api>";

        private static string QueryReply(string content)
            => ApiReply($"<query>{content}</query>");
    }
}
