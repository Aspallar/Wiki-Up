using System.Collections.Generic;

namespace WikiUpload
{
    public interface IUploadResponse
    {
        string ArchiveDuplicate { get; }
        List<string> Duplicates { get; }
        bool IsDuplicate { get; }
        bool IsDuplicateOfArchive { get; }
        IReadOnlyResponseErrors Errors { get; }
        string Result { get; }
        int RetryDelay { get; }
        IReadOnlyList<string> Warnings { get; }
        string WarningsText { get; }
        string Xml { get; }
    }
}