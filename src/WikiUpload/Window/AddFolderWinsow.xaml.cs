using System.Globalization;
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
            Loaded += AddFolderWindow_Loaded;
        }

        private void AddFolderWindow_Loaded(object sender, RoutedEventArgs e)
        {
            switch (CultureInfo.CurrentUICulture.Name)
            {
                case "fr-FR":
                    IncludeFileOfType.Width = 460;
                    break;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
