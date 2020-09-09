using System.Security;

namespace WikiUpload
{
    public interface IPasswordManager
    {
        void SavePassword(string site, string username, SecureString password);

        char[] GetPassword(string site, string username);

        void RemovePassword(string site, string username);
        
        bool HasPassword(string site, string username);
    }
}
