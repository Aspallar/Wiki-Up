using System.Security;
using WikiUpload.Utilities;

namespace WikiUpload
{
    public interface IPasswordManager
    {
        void SavePassword(string site, string username, SecureString password);

        SecureCharArray GetPassword(string site, string username);

        void RemovePassword(string site, string username);
        
        bool HasPassword(string site, string username);
    }
}
