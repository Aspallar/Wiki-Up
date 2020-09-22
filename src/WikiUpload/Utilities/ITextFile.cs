namespace WikiUpload
{
    public interface ITextFile
    {
        string ReadAllText(string path);
        void WriteAllText(string path, string content);
    }
}