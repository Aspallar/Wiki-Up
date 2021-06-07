namespace WikiUpload
{
    internal interface IReadOnlyPermittedFiles
    {
        string[] GetExtensions();
        bool IsPermitted(string extension);
    }
}
