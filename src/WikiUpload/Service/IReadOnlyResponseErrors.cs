namespace WikiUpload
{
    internal interface IReadOnlyResponseErrors
    {
        bool IsAny { get; }
        bool IsMutsBeLoggedInError { get; }
        bool IsTokenError { get; }
        bool IsRateLimitedError { get; }

        string ToString();
    }
}