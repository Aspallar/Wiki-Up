namespace TestServer
{
    internal class ServerResponseHeader
    {
        public string Name { get; }
        public string Value { get; }

        public ServerResponseHeader(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
