namespace WikiUpload
{
    internal interface IUploadResponse
    {
        IReadOnlyResponseErrors Errors { get; }
        IReadOnlyResponseWarnings Warnings { get; }
        string Result { get; }
        int RetryDelay { get; }
    }
}