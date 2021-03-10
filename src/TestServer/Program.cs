using CommandLine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace TestServer
{
    class Program
    {
        const string loginToken = "123456+\\";

        static readonly object randLock = new object();
        static readonly Random rand = new Random();
        static long videoUploadCount = 0;

        static void Main(string[] args)
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
                Console.WriteLine(context.Request.RawUrl);
                ThreadPool.QueueUserWorkItem(x => HandleRequest(context, options));
            }
        }

        private static int GetRandom(int max)
        {
            lock (randLock) return rand.Next(max);
        }

        private static void HandleRequest(HttpListenerContext context, Options options)
        {
            string reply; ;
            var request = context.Request;
            var response = context.Response;
            int statusCode = 200;

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
                        VideoUploadReply(ref reply, ref statusCode);
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
            byte[] buffer = Encoding.UTF8.GetBytes(reply);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            if (options.ShowReply)
                Console.WriteLine($"\nResponse Sent:\n{reply}\n");
        }

        private static void VideoUploadReply(ref string reply, ref int statusCode)
        {
            var count = Interlocked.Increment(ref videoUploadCount) % 5;
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
                reply = QueryReply("<users><user><groups><g>autoconfirmed</g></groups></user></users>");

            else if (request.RawUrl.IndexOf("6EA4096B-EBD2-4B9D-9025-2BA38D336E43") != -1)
                reply = QueryReply("<pages><page edittoken=\"666+\\\"></page></pages>");

            else if (HttpUtility.UrlDecode(request.RawUrl).IndexOf("MediaWiki:Custom-WikiUpUsers") != -1)
                reply = QueryReply("<pages><page><revisions><rev xml:space=\"preserve\">a\nb\nc\naspallar\nfoo\nbar\n</rev></revisions></page></pages>");

            else if (request.RawUrl.IndexOf("meta=siteinfo&siprop=fileextensions") != -1)
                reply = options.NoPermittedFiles
                    ? ApiReply("")
                    : QueryReply("<fileextensions><fe ext=\"png\" /><fe ext=\"jpg\" /><fe ext=\"foo\" /></fileextensions>");

            else if (request.RawUrl.ContainsAll(new List<string> { "meta=tokens", "type=login" }))
                reply = options.OldLogin
                    ? QueryReply("<warnings><query xml:space=\"preserve\">Unrecognized value for parameter 'meta': tokens</query></warnings>")
                    : QueryReply($"<tokens logintoken=\"{loginToken}\" />");

            else if (request.RawUrl.IndexOf("meta=tokens&type=csrf") != -1)
                reply = QueryReply("<tokens csrftoken=\"666+\\\" />");

            else if (request.RawUrl.IndexOf("allcategories") != -1)
                reply = null;

            else
                reply = ApiReply("");

            return reply;
        }

        private static string UploadReply(Options options, HttpListenerResponse response)
        {
            string reply;
            if (options.MaxLag == -1)
                reply = MaxLagReply(response);
            else if (options.Exists > 0 && GetRandom(100) < options.Exists)
                reply = ApiReply("<upload result=\"Warning\"><warnings exists=\"\"></warnings></upload>");
            else if (options.InvalidXml > 0 && GetRandom(100) < options.InvalidXml)
                reply = "Hey this is not xml";
            else if (options.MaxLag > 0 && GetRandom(100) < options.MaxLag)
                reply = MaxLagReply(response);
            else if (options.LongError > 0 && GetRandom(100) < options.LongError)
                reply = ApiReply("<error code=\"foobar\" info=\"This os a long error message. Lorem ipsum dolor sit amet, consectetur adipiscing elit.Vivamus pretium neque et arcu scelerisque, vel accumsan ipsum elementum.Sed in convallis tortor.Morbi mollis nunc et felis pharetra, a pellentesque lectus volutpat.Aliquam eleifend purus purus, nec laoreet mi vestibulum. Once upon a time. And then there wrere none. The end.\"></error>");
            else
                reply = ApiReply("<upload result=\"Success\"></upload>");

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
                        reply = ApiReply("<login result=\"Success\" />");
                    else
                        reply = ApiReply("<login result=\"WrongPass\" />");
                }
                else
                {
                    reply = ApiReply($"<login result=\"NeedToken\" token=\"{loginToken}\" />");
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
            return ApiReply("<error code=\"maxlag\" info=\"Waiting for a database server: 6 seconds lagged.\" host=\"foobar\" lag=\"6\" type=\"db\"></error>");
        }

        private static string ApiReply(string content)
            => $"<?xml version=\"1.0\"?><api>{content}</api>";

        private static string QueryReply(string content)
            => ApiReply($"<query>{content}</query>");
    }
}
