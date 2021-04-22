using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class AddFolderOptionsViewModel : WindowViewModel
    {
        public AddFolderOptionsViewModel(Window window, IFileUploader fileUploader) : base(window)
        {
            var permittedFiles = fileUploader.PermittedFiles.GetExtensions();
            var fileTypes = new List<string>
            {
                Resources.IncludeAllFiles,
                Resources.IncludeUploadableFiles,
                Resources.IncludeImageFiles
            };
            foreach (var item in permittedFiles.Select(x => x[1..].ToUpper()).OrderBy(x => x))
                fileTypes.Add(item);

            FileTypes = fileTypes;
            SelectedFileTypeIndex = 1;
            IncludeSubfolders = true;
        }

        public int SelectedFileTypeIndex { get; set; }

        public List<string> FileTypes { get; set; }

        public bool IncludeSubfolders { get; set; }

        public string FolderPath { get; set; }

        public IncludeFiles GetIncludeFiles()
        {
            return SelectedFileTypeIndex switch
            {
                0 => IncludeFiles.All,
                1 => IncludeFiles.Uploadable,
                2 => IncludeFiles.Image,
                _ => IncludeFiles.SingleExtension,
            };
        }

        public string GetExtension()
            => SelectedFileTypeIndex > 2 ? FileTypes[SelectedFileTypeIndex] : "";
    }

}
