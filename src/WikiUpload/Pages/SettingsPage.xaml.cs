using System;
using System.Windows.Controls;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void ImageFileExtensionPopu_Opened(object sender, EventArgs e)
        {
            ImageExtension.Focus();
            ImageExtension.SelectAll();
        }
    }
}
