using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    public class ErrorMessageViewModel : WindowViewModel
    {
        public ErrorMessageViewModel(Window window) : base(window)
        {
            TitleHeight = 26;
        }

        public string ErrorMessage { get; set; }
        
        public string ExceptonMessage { get; set; }

        public Visibility ExceptionVisibility
            => string.IsNullOrEmpty(ExceptonMessage) ? Visibility.Collapsed : Visibility.Visible;

    }
}
