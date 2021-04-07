using System.IO;
using System.Text.RegularExpressions;

namespace WikiUpload
{
    public class VariableContent
    {
        private readonly Regex _filepartRegex = new Regex(@"<%(filename|-?\d+)>");

        private readonly string _content;

        public bool HasVariables { get; }

        public VariableContent(string content)
        {
            _content = content;
            HasVariables = _filepartRegex.IsMatch(content);
        }

        public string ExpandedContent(UploadFile file)
        {
            return HasVariables ? ExpandContent(file) : _content;
        }

        private string ExpandContent(UploadFile file)
        {
            var pathParts = file.FullPath.Split(Path.DirectorySeparatorChar);
            var fileName = Path.GetFileNameWithoutExtension(file.FullPath);
            return _filepartRegex.Replace(_content, (match) =>
            {
                var what = match.Groups[1].Value;
                if (int.TryParse(what, out var index))
                    return PathPart(file, pathParts, index);
                else if (what == "filename")
                    return fileName;
                else
                    return match.Value;
            });
        }

        private static string PathPart(UploadFile file, string[] pathParts, int index)
        {
            if (index < 0)
            {
                index = pathParts.Length + index + 1;
                if (index <= 0)
                    return "";
            }

            if (index == 0)
                return file.FullPath;
            else if (index <= pathParts.Length)
                return pathParts[index - 1];
            else
                return "";
        }
    }
}
