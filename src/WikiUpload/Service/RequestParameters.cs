using System.Collections.Generic;
using System.Net;
using System.Text;

namespace WikiUpload
{
    internal class RequestParameters : List<KeyValuePair<string, string>>
    {
        public RequestParameters() : base() { }
        
        public void Add(string name, string value)
            => base.Add(new KeyValuePair<string, string>(name, value));

        public override string ToString()
        {
            var queryString = new StringBuilder();
            if (Count > 0)
            {
                queryString.Append('?');
                foreach (var param in this)
                {
                    queryString.Append(WebUtility.UrlEncode(param.Key));
                    queryString.Append('=');
                    queryString.Append(WebUtility.UrlEncode(param.Value));
                    queryString.Append('&');
                }
                queryString.Length -= 1;
            }
            return queryString.ToString();
        }
    }
}
