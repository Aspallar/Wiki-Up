using System.Text;
using System.Windows.Input;

namespace WikiUpload
{
    internal class UploadedFilesViewModel : BaseViewModel
    {
        private IFileUploader _fileUploader;
        private IHelpers _helpers;

        public UploadedFilesViewModel(IFileUploader fileUploader, IHelpers helpers)
        {
            _fileUploader = fileUploader;
            _helpers = helpers;

            UploadedFiles = new UploadList(_helpers);
            LaunchFilePageCommand = new RelayParameterizedCommand((file) => LaunchFilePage((UploadFile)file));
            CopyToClipboardCommand = new RelayCommand(CopyToClipboard);
        }

        public UploadList UploadedFiles { get; }

        public ICommand LaunchFilePageCommand { get; }

        private void LaunchFilePage(UploadFile file)
        {
            var url = _fileUploader.FileUrl(file.FileName);
            _helpers.LaunchProcess(url);
        }

        public ICommand CopyToClipboardCommand { get; }
        private void CopyToClipboard()
        {
            var output = new StringBuilder();
            foreach (var item in UploadedFiles)
                output.AppendLine(_fileUploader.ServerFilename(item.FileName));
            _helpers.SetClipboardText(output.ToString());
        }

    }
}
 