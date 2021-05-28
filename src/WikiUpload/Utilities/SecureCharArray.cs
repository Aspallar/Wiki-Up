using System;

namespace WikiUpload.Utilities
{
    internal sealed class SecureCharArray : IDisposable
    {
        private readonly char[] _array;

        public SecureCharArray(char[] array)
        {
            _array = array;
        }

        public char[] Data => _array;

        public void Dispose()
        {
            Array.Clear(_array, 0, _array.Length);
        }
    }
}
