using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task AddNewRangeAsync(IList<string> items)
        {
            var k = 0;
            foreach (var item in items)
            {
                AddIfNotDuplicate(new UploadFile { FullPath = item });
                if (++k == 50)
                {
                    k = 0;
                    await Task.Delay(10);
                }
            }
        }

        public void AddRange(IList<UploadFile> items)
        {
            foreach (var item in items)
                AddIfNotDuplicate(item);
        }
    }
}
