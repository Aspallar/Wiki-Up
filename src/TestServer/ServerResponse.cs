using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TestServer
{
    internal class ServerResponse
    {
        public int StatusCode { get; set; } = 200;

        public string Reply { get; set; } = "";

        public bool TimeoutRequest { get; set; }

        public List<ServerResponseHeader> Headers { get; } = new List<ServerResponseHeader>();

        public void Send(HttpListenerResponse response)
        {
            try
            {
                response.StatusCode = StatusCode;
                foreach (var header in Headers)
                    response.AddHeader(header.Name, header.Value);
                var buffer = Encoding.UTF8.GetBytes(Reply);
                response.ContentLength64 = buffer.Length;
                response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            finally
            {
                response.OutputStream.Close();
            }
        }

    }
}
