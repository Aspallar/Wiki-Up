namespace WikiUpload
{
    public class Language
    {
        public string Name { get; set; }
        public string Code { get; set; }

        public Language() { }

        public Language(string name, string code)
        {
            Name = name;
            Code = code;
        }
    }
}
