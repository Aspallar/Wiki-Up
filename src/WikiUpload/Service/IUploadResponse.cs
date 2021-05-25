using System.Collections.Generic;

namespace WikiUpload
{
    public interface IUploadResponse
    {
        IReadOnlyResponseErrors Errors { get; }
        IReadOnlyResponseWarnings Warnings { get; }
        string Result { get; }
        int RetryDelay { get; }
    }
}