namespace WikiUpload
{
    internal class ApiError
    {
        public string Code { get; private set; }
        public string Info { get; private set; }

        public ApiError(string code, string info)
        {
            Code = code;
            Info = info;
        }
    }
}
