using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading.Tasks;

namespace WikiUpload
{
    internal static class SecureStringExtensions
    {
        #region Taken from https://stackoverflow.com/a/61285569

        /// <remarks>
        /// This method creates an empty managed string and pins it so that the garbage collector
        /// cannot move it around and create copies. An unmanaged copy of the the secure string is
        /// then created and copied into the managed string. The action is then called using the
        /// managed string. Both the managed and unmanaged strings are then zeroed to erase their
        /// contents. The managed string is unpinned so that the garbage collector can resume normal
        /// behaviour and the unmanaged string is freed.
        /// </remarks>
        public static T UseUnsecuredString<T>(this SecureString secureString, Func<string, T> action)
        {
            var length = secureString.Length;
            var sourceStringPointer = IntPtr.Zero;

            // Create an empty string of the correct size and pin it so that the GC can't move it around.
            var insecureString = new string('\0', length);
            var insecureStringHandler = GCHandle.Alloc(insecureString, GCHandleType.Pinned);

            var insecureStringPointer = insecureStringHandler.AddrOfPinnedObject();

            try
            {
                // Create an unmanaged copy of the secure string.
                sourceStringPointer = Marshal.SecureStringToBSTR(secureString);

                // Use the pointers to copy from the unmanaged to managed string.
                for (var i = 0; i < secureString.Length; i++)
                {
                    var unicodeChar = Marshal.ReadInt16(sourceStringPointer, i * 2);
                    Marshal.WriteInt16(insecureStringPointer, i * 2, unicodeChar);
                }

                return action(insecureString);
            }
            finally
            {
                // Zero the managed string so that the string is erased. Then unpin it to allow the
                // GC to take over.
                Marshal.Copy(new byte[length * 2], 0, insecureStringPointer, length * 2);
                insecureStringHandler.Free();

                // Zero and free the unmanaged string.
                Marshal.ZeroFreeBSTR(sourceStringPointer);
            }
        }


        public static async Task<T> UseUnsecuredStringAsync<T>(this SecureString secureString, Func<string, Task<T>> action)
        {
            var length = secureString.Length;
            var sourceStringPointer = IntPtr.Zero;

            // Create an empty string of the correct size and pin it so that the GC can't move it around.
            var insecureString = new string('\0', length);
            var insecureStringHandler = GCHandle.Alloc(insecureString, GCHandleType.Pinned);

            var insecureStringPointer = insecureStringHandler.AddrOfPinnedObject();

            try
            {
                // Create an unmanaged copy of the secure string.
                sourceStringPointer = Marshal.SecureStringToBSTR(secureString);

                // Use the pointers to copy from the unmanaged to managed string.
                for (var i = 0; i < secureString.Length; i++)
                {
                    var unicodeChar = Marshal.ReadInt16(sourceStringPointer, i * 2);
                    Marshal.WriteInt16(insecureStringPointer, i * 2, unicodeChar);
                }

                return await action(insecureString);
            }
            finally
            {
                // Zero the managed string so that the string is erased. Then unpin it to allow the
                // GC to take over.
                Marshal.Copy(new byte[length * 2], 0, insecureStringPointer, length * 2);
                insecureStringHandler.Free();

                // Zero and free the unmanaged string.
                Marshal.ZeroFreeBSTR(sourceStringPointer);
            }
        }


        /// <summary>
        /// Allows a decrypted secure string to be used whilst minimising the exposure of the
        /// unencrypted string.
        /// </summary>
        /// <param name="secureString">The string to decrypt.</param>
        /// <param name="action">
        /// Func delegate which will receive the decrypted password as a string object
        /// </param>
        /// <returns>Result of Func delegate</returns>
        /// <remarks>
        /// This method creates an empty managed string and pins it so that the garbage collector
        /// cannot move it around and create copies. An unmanaged copy of the the secure string is
        /// then created and copied into the managed string. The action is then called using the
        /// managed string. Both the managed and unmanaged strings are then zeroed to erase their
        /// contents. The managed string is unpinned so that the garbage collector can resume normal
        /// behaviour and the unmanaged string is freed.
        /// </remarks>
        public static void UseUnsecuredString(this SecureString secureString, Action<string> action)
        {
            UseUnsecuredString(secureString, (s) =>
            {
                action(s);
                return 0;
            });
        }

        #endregion
    }
}
