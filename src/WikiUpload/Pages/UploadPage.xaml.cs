namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for UploadPage.xaml
    /// </summary>
    public partial class UploadPage : BasePage<UploadViewModel>
    {
        public UploadPage()
        {
            InitializeComponent();
        }

        private void AddCategory_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            const string enterCategory = "Enter Category Name";
            ContentText.Text += $"\n[[Category:{enterCategory}]]";
            ContentText.Select(ContentText.Text.Length - enterCategory.Length - 2, enterCategory.Length);
            ContentText.ScrollToEnd();
            ContentText.Focus();
        }
    }
}
