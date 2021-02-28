using System;
using System.Media;
using System.Windows;

namespace WikiUpload
{
    public partial class ErrorMessageWindow : Window
    {
        public ErrorMessageWindow(string errorMessage, string subMessage)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            
            var viewModel = App.ServiceLocator.ErrorMessageViewModel(this);
            viewModel.ErrorMessage = errorMessage;
            viewModel.SubMessage = subMessage;

            DataContext = viewModel;

            Loaded += (s, e) =>  SystemSounds.Beep.Play();
        }
    }
}
