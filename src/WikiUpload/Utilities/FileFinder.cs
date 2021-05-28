using System.Collections.Generic;
using System.IO;
using System.Linq;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class FileFinder : IFileFinder
    {
        private readonly IFileUploader _fileUploader;
        private readonly IAppSettings _appSettings;
        private readonly IHelpers _helpers;

        public FileFinder(
            IFileUploader fileUploader,
            IAppSettings appSettings,
            IHelpers helpers)
        {
            _fileUploader = fileUploader;
            _appSettings = appSettings;
            _helpers = helpers;
        }

        public IEnumerable<string> GetFiles(string folderPath, bool includeSubfolders, IncludeFiles includeFiles, string extension)
        {
            var pattern = includeFiles == IncludeFiles.SingleExtension ? "*." + extension : "*";
            var searchOption = includeSubfolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var fileNames = _helpers.EnumerateFiles(folderPath, pattern, searchOption);
            if (includeFiles == IncludeFiles.Uploadable || includeFiles == IncludeFiles.Image)
            {
                fileNames = fileNames.Where(x => _fileUploader.PermittedFiles.IsPermitted(x));
                if (includeFiles == IncludeFiles.Image)
                {
                    var imageExtensions = _appSettings.ImageExtensions.Split(';').Select(x => "." + x.ToUpperInvariant()).ToList();
                    fileNames = fileNames.Where(x => imageExtensions.Contains(Path.GetExtension(x).ToUpperInvariant()));
                }
            }
            return fileNames;
        }
    }
}
