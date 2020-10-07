using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace WikiUpload
{
    [AddINotifyPropertyChangedInterface]
    public abstract class WikiSearch : INotifyPropertyChanged, IWikiSearch
    {
        private Stack<string> _history = new Stack<string>();
        private string _nextFrom = "";
        private int _multipleRequestGuard = 0;

        public abstract Task<SearchResponse> FetchData(string from);
        public abstract string FullItemString(string item);

        public async Task Start(string from)
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                try
                {
                    _history.Clear();
                    _nextFrom = from;
                    await DoNext();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                finally
                {
                    Interlocked.Decrement(ref _multipleRequestGuard);
                }
            }
        }

        public async Task Next()
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                try
                {
                    await DoNext();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                finally
                {
                    Interlocked.Decrement(ref _multipleRequestGuard);
                }
            }
        }

        private async Task DoNext()
        {
            IsError = false;
            var response = await FetchData(_nextFrom);
            _history.Push(_nextFrom);
            _nextFrom = response.NextFrom;
            HasNext = !string.IsNullOrEmpty(response.NextFrom);
            CalculateHasPrevious();
            Data = response.Categories;
        }

        public async Task Previous()
        {
            if (_multipleRequestGuard == 0)
            {
                Interlocked.Increment(ref _multipleRequestGuard);
                try
                {
                    IsError = false;
                    _history.Pop();
                    var previous = _history.Peek();
                    var response = await FetchData(previous);
                    _nextFrom = response.NextFrom;
                    Data = response.Categories;
                    HasNext = true;
                    CalculateHasPrevious();
                }
                catch (Exception ex)
                {
                    HandleError(ex);
                }
                finally
                {
                    Interlocked.Decrement(ref _multipleRequestGuard);
                }
            }
        }

        private void HandleError(Exception ex)
        {
            if (ex is TaskCanceledException)
                ErrorMessage = "The server took too long to respond.";
            else if (ex.InnerException == null)
                ErrorMessage = ex.Message;
            else
                ErrorMessage = ex.InnerException.Message;
            IsError = true;
        }

        private void CalculateHasPrevious() => HasPrevious = _history.Count > 1;

        public bool HasNext { get; private set; }

        public bool HasPrevious { get; private set; }

        public bool IsError { get; private set; }

        public string ErrorMessage { get; private set; }

        public List<string> Data { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public void OnPropertyChanged(string name)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
