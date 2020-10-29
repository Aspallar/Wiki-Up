using System;
using System.Media;
using System.Runtime.InteropServices;
using System.Windows;

namespace WikiUpload
{
    public partial class ErrorMessageWindow : Window
    {
        public ErrorMessageWindow(string errorMessage, Exception ex)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            
            var viewModel = App.ServiceLocator.ErrorMessageViewModel(this);
            viewModel.ErrorMessage = errorMessage;
            if (ex != null)
                viewModel.ExceptonMessage = ex.Message;

            DataContext = viewModel;

            Loaded += (s, e) =>  SystemSounds.Beep.Play();
        }
    }
}
