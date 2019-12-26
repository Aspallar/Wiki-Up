namespace WikiUpload
{
    public interface IReadOnlyPermittedFiles
    {
        string[] GetExtensions();
        bool IsPermitted(string extension);
    }
}
