using System.Windows;

namespace WikiUpload
{
    internal class ErrorMessageViewModel : WindowViewModel
    {
        public ErrorMessageViewModel(Window window) : base(window)
        {
            TitleHeight = 26;
        }

        public string ErrorMessage { get; set; }
        
        public string SubMessage { get; set; }

        public bool HasCancelButton { get; set; }
    }
}
