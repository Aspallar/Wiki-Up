using System.IO;

namespace WikiUpload
{
    public class TextFile : ITextFile
    {
        public string ReadAllText(string path) => File.ReadAllText(path);

        public void WriteAllText(string path, string content) => File.WriteAllText(path, content);
    }
}
