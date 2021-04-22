using System;
using System.IO;

namespace WikiUpload
{
    public class ExtensionValidater
    {
        private readonly char[] _invalidExtensionCharacters;

        public ExtensionValidater()
        {
            var invalidChars = Path.GetInvalidFileNameChars();
            _invalidExtensionCharacters = new char[invalidChars.Length + 1];
            Array.Copy(invalidChars, _invalidExtensionCharacters, invalidChars.Length);
            _invalidExtensionCharacters[^1] = ';';
        }

        public bool IsValid(string extension)
            => extension.IndexOfAny(_invalidExtensionCharacters) == -1;
    }
}
