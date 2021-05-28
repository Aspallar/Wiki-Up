using System.Security;

namespace WikiUpload
{
    /// <summary>
    /// An interface for a class that can provide a secure password
    /// </summary>
    internal interface IHavePassword
    {
        /// <summary>
        /// The secure password
        /// </summary>
        SecureString SecurePassword { get; }
    }
}

