using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace WikiUpload
{
    public class UploadList : ObservableCollection<UploadFile>
    {
        private const int chunkSize = 30;
        private const int chunkDelay = 1;
        
        private readonly IHelpers _helpers;

        public UploadList(IHelpers helpers) : base()
        {
            _helpers = helpers;
        }

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

        public void AddNewRange(IEnumerable<string> items)
        {
            foreach (var item in items)
                AddIfNotDuplicate(new UploadFile(item));
        }

        public async Task AddNewRangeAsync(IEnumerable<string> items)
        {
            var k = 0;
            foreach (var item in items)
            {
                AddIfNotDuplicate(new UploadFile(item));
                if (++k == chunkSize)
                {
                    k = 0;
                    await _helpers.Wait(chunkDelay);
                }
            }
        }

        public void AddRange(IEnumerable<UploadFile> items)
        {
            foreach (var item in items)
                AddIfNotDuplicate(item);
        }

        public async Task AddRangeAsync(IEnumerable<UploadFile> items)
        {
            var k = 0;
            foreach (var item in items)
            {
                AddIfNotDuplicate(item);
                if (++k == chunkSize)
                {
                    k = 0;
                    await _helpers.Wait(chunkDelay);
                }
            }
        }


    }
}
