using System.Windows.Controls;

namespace WikiUpload
{
    public class BasePage<T> : Page
        where T : BaseViewModel, new()
    {
        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        private T _viewModel;

        /// <summary>
        /// The View Model associated with this page
        /// </summary>
        public T ViewModel
        {
            get { return _viewModel; }
            set
            {
                if (_viewModel != value)
                {
                    _viewModel = value;
                    this.DataContext = _viewModel;
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BasePage()
        {
            this.ViewModel = new T();
        }
    }
}
