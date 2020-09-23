using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WikiUpload
{
    public class UploadList : ObservableCollection<IUploadFile>
    {
        public void RemoveRange(IList<UploadFile> itemsToRemove)
        {
            foreach (var item in itemsToRemove)
                Remove(item);
        }

        public void AddNewRange(IList<string> items)
        {
            foreach (var item in items)
                Add(new UploadFile { FullPath = item });
        }

        public void AddRange(IList<UploadFile> items)
        {
            foreach (var item in items)
                Add(item);
        }
    }
}
