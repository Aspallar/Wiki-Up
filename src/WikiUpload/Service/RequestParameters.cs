using System.Collections.Generic;

namespace WikiUpload
{
    internal class RequestParameters : List<KeyValuePair<string, string>>
    {
        public RequestParameters() : base() { }

        public void Add(string name, string value)
        {
            base.Add(new KeyValuePair<string, string>(name, value));
        }
    }
}
