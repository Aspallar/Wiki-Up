using System.Collections.Generic;
using System.IO;

namespace WikiUpload
{
    public class PermittedFiles : IReadOnlyPermittedFiles
    {
        private readonly object _copyLock = new();
        private readonly List<string> _extensions = new();

        public PermittedFiles Add(string extension)
        {
            _extensions.Add("." + extension);
            return this;
        }

        public bool IsPermitted(string fileName)
            => _extensions.Count == 0 || _extensions.Contains(Path.GetExtension(fileName));

        public string[] GetExtensions()
        {
            lock (_copyLock)
            {
                var result = new string[_extensions.Count];
                _extensions.CopyTo(result);
                return result;
            }
        }

        public void Clear()
        {
            _extensions.Clear();
        }
    }
}
