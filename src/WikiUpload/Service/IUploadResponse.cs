using System.Collections.Generic;

namespace WikiUpload
{
    public interface IUploadResponse
    {
        string ArchiveDuplicate { get; }
        List<string> Duplicates { get; }
        IReadOnlyList<ApiError> Errors { get; }
        string ErrorsText { get; }
        bool IsDuplicate { get; }
        bool IsDuplicateOfArchive { get; }
        bool IsError { get; }
        bool IsTokenError { get; }
        string Result { get; }
        int RetryDelay { get; }
        IReadOnlyList<string> Warnings { get; }
        string WarningsText { get; }
        string Xml { get; }
    }
}