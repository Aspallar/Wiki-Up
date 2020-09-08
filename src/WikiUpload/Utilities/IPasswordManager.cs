using System.Security;

namespace WikiUpload
{
    public interface IPasswordManager
    {
        void SavePassword(string site, string username, SecureString password);

        string GetPassword(string site, string username);

        void RemovePassword(string site, string username);
    }
}
