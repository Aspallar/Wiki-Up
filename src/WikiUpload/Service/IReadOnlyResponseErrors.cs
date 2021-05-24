namespace WikiUpload
{
    public interface IReadOnlyResponseErrors
    {
        bool IsAny { get; }
        bool IsMutsBeLoggedInError { get; }
        bool IsTokenError { get; }
        string ToString();
    }
}