using System.Windows.Input;

namespace WikiUpload
{
    public interface IUploadFileCommands
    {
        ICommand EditUploadFileNameCommand { get; }
        ICommand ShowFileCommand { get; }
    }
}
