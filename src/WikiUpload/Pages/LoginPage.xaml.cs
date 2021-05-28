using System.Security;
using System.Windows.Controls;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    internal partial class LoginPage : Page, IHavePassword
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The secure password for this login page
        /// </summary>
        public SecureString SecurePassword => PasswordText.SecurePassword;
    }
}
