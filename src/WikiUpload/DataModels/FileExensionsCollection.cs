using System;
using System.Collections.ObjectModel;
using System.Text;

namespace WikiUpload
{
    public class FileExensionsCollection : ObservableCollection<string>
    {
        public FileExensionsCollection() : base()  { }

        public FileExensionsCollection(string semiColonSeparatedSrring) : base()
        {
            var semiColon = new char[] { ';' };
            foreach (var s in semiColonSeparatedSrring.Split(semiColon, StringSplitOptions.RemoveEmptyEntries))
                Add(s);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var s in this)
                sb.Append(s).Append(';');
            return sb.ToString();
        }
    }
}
