namespace WikiUpload
{
    internal class ColorTheme
    {
        public Skin Id { get; set; }
        public string Name { get; set; }

        public ColorTheme()
        {
        }

        public ColorTheme(Skin id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}