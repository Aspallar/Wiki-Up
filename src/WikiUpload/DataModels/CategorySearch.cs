using PropertyChanged;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    [AddINotifyPropertyChangedInterface]
    public class CategorySearch : INotifyPropertyChanged
    {
        private Stack<string> _history = new Stack<string>();
        private IFileUploader _fileUploader;
        private string _nextFrom = "";
        private int _multipleRequestGuard = 0;

        public CategorySearch(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public async Task Start(string from)
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                _history.Clear();
                _nextFrom = from;
                await DoNext();
                Interlocked.Decrement(ref _multipleRequestGuard);
            }
        }

        public async Task Next()
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                await DoNext();
                Interlocked.Decrement(ref _multipleRequestGuard);
            }
        }

        private async Task DoNext()
        {
            var response = await _fileUploader.FetchCategories(_nextFrom);
            _history.Push(_nextFrom);
            _nextFrom = response.NextFrom;
            HasNext = !string.IsNullOrEmpty(response.NextFrom);
            CalculateHasPrevious();
            Categories = response.Categories;
        }

        public async Task Previous()
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                _history.Pop();
                var prev = _history.Peek();
                var response = await _fileUploader.FetchCategories(prev);
                _nextFrom = response.NextFrom;
                Categories = response.Categories;
                HasNext = true;
                CalculateHasPrevious();
                Interlocked.Decrement(ref _multipleRequestGuard);
            }
        }

        private void CalculateHasPrevious() => HasPrevious = _history.Count > 1;

        public bool HasNext { get; private set; }

        public bool HasPrevious { get; private set; }

        public List<string> Categories { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
