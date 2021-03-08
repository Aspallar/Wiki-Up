using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WikiUpload
{
    public class UploadList : ObservableCollection<UploadFile>
    {
        public void RemoveRange(IList<UploadFile> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
                Remove(item);
        }

        public void AddIfNotDuplicate(UploadFile file)
        {
            if (!this.Any(x => x.FullPath == file.FullPath))
                base.Add(file);
        }

        public void AddNewRange(IList<string> items)
        {
            foreach (var item in items)
                AddIfNotDuplicate(new UploadFile { FullPath = item });
        }

        public void AddRange(IList<UploadFile> items)
        {
            foreach (var item in items)
                AddIfNotDuplicate(item);
        }
    }
}
