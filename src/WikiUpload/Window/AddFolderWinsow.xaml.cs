using System.Windows;

namespace WikiUpload
{
    internal partial class AddFolderWindow : Window
    {
        public AddFolderWindow(string folderPath)
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            var context = App.ServiceLocator.AddFolderOptionsViewModel(this);
            context.FolderPath = folderPath;
            DataContext = context;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
