using System.Collections.Generic;

namespace WikiUpload
{
    internal interface IFileFinder
    {
        IEnumerable<string> GetFiles(string folderPath, bool includeSubfolders, IncludeFiles includeFiles, string extension);
    }
}