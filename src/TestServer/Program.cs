using CommandLine;
using System;
using System.Net;
using System.Text;
using System.Threading;

namespace TestServer
{
    class Program
    {
        static readonly object randLock = new object();
        static readonly Random rand = new Random();

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
            string reply = ApiReply("");
            var request = context.Request;
            var response = context.Response;

            //Console.WriteLine($"RawUrl: {request.RawUrl}");
            if (request.HasEntityBody)
            {
                if (request.ContentType == "application/x-www-form-urlencoded")
                {
                    if (options.LoginTimeout)
                        return;
                    if (options.InvalidLoginXml)
                        reply = "Howdy = I'm not xml";
                    else
                        reply = ApiReply("<login result=\"Success\" />");
                }
                else if (request.ContentType.StartsWith("multipart/form-data"))
                {
                    if (options.UploadTimeout)
                        return;
                    //Console.WriteLine("Type: Image upload");
                    if (options.MaxLag == -1)
                        reply = MaxLagReply(response);
                    else if (options.Exists > 0 && GetRandom(100) < options.Exists)
                        reply = ApiReply("<upload result=\"Warning\"><warnings exists=\"\"></warnings></upload>");
                    else if (options.InvalidXml > 0 && GetRandom(100) < options.InvalidXml)
                        reply = "Hey this is not xml";
                    else if (options.MaxLag > 0 && GetRandom(100) < options.MaxLag)
                        reply = MaxLagReply(response);
                    else
                        reply = ApiReply("<upload result=\"Success\"></upload>");

                    if (options.Delay > 0)
                        Thread.Sleep(options.Delay);
                }
                request.InputStream.Close();
            }
            else
            {
                if (request.RawUrl.IndexOf("list=users&usprop=groups&ususers") != -1)
                    reply = QueryReply("<users><user><groups><g>autoconfirmed</g></groups></user></users>");

                else if (request.RawUrl.IndexOf("&prop=info&intoken=edit&titles=Foo") != -1)
                    reply = QueryReply("<pages><page edittoken=\"666+\\\"></page></pages>");

                else if (request.RawUrl.IndexOf("MediaWiki:Custom-WikiUploadUsers") != -1)
                    reply = QueryReply("<pages><page><revisions><rev xml:space=\"preserve\">a\nb\nc\naspallar\nfoo\nbar\n</rev></revisions></page></pages>");

                else if (request.RawUrl.IndexOf("meta=siteinfo&siprop=fileextensions") != -1)
                    reply = QueryReply("<fileextensions><fe ext=\"png\" /><fe ext=\"jpg\" /></fileextensions>");

                else if (request.RawUrl.IndexOf("meta=tokens&type=login") != -1)
                    reply = QueryReply("<tokens logintoken=\"666+\\\" />");

                else if (request.RawUrl.IndexOf("meta=tokens&type=csrf") != -1)
                    reply = QueryReply("<tokens csrftoken=\"666+\\\" />");
            }

            response.StatusCode = 200;
            byte[] buffer = Encoding.UTF8.GetBytes(reply);
            response.ContentLength64 = buffer.Length;
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            if (options.ShowReply)
                Console.WriteLine($"\nResponse Sent:\n{reply}\n");
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
