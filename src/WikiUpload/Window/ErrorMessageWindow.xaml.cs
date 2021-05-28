using System.Media;
using System.Windows;

namespace WikiUpload
{
    internal partial class ErrorMessageWindow : Window
    {
        public ErrorMessageWindow(string errorMessage, string subMessage, bool hasCancelVutton = false)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            
            var viewModel = App.ServiceLocator.ErrorMessageViewModel(this);
            viewModel.ErrorMessage = errorMessage;
            viewModel.SubMessage = subMessage;
            viewModel.HasCancelButton = hasCancelVutton;

            DataContext = viewModel;

            Loaded += (s, e) =>  SystemSounds.Beep.Play();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
