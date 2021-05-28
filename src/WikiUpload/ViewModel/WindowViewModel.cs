using System.Windows;
using System.Windows.Input;

namespace WikiUpload
{
    internal class WindowViewModel : BaseViewModel
    {
        public WindowViewModel(Window window)
        {
            InitializeCustomWindowChrome(window);
        }
        

        #region Custom Window Chrome Support

        private void InitializeCustomWindowChrome(Window window)
        {
            _window = window;

            _window.StateChanged += (sender, e) =>
            {
                WindowResized();
            };

            MinimizeCommand = new RelayCommand(() => _window.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(() => _window.WindowState ^= WindowState.Maximized);
            CloseCommand = new RelayCommand(() => _window.Close());
            MenuCommand = new RelayCommand(() => SystemCommands.ShowSystemMenu(_window, GetMousePosition()));

            // Fix window resize issueS
            var resizer = new WindowResizer(_window);

            // Listen out for dock changes
            resizer.WindowDockChanged += (dock) =>
            {
                // Store last position
                _dockPosition = dock;

                // Fire off resize events
                WindowResized();
            };
        }

        public ICommand MinimizeCommand { get; set; }

        public ICommand MaximizeCommand { get; set; }

        public ICommand CloseCommand { get; set; }

        public ICommand MenuCommand { get; set; }
        
        protected Window _window;

        private int _outerMarginSize = 10;

        private int _windowRadius = 8;

        private WindowDockPosition _dockPosition = WindowDockPosition.Undocked;

        public double WindowMinimumWidth => 423;

        public double WindowMinimumHeight => 64;

        private bool Borderless => _window.WindowState == WindowState.Maximized || _dockPosition != WindowDockPosition.Undocked;

        public int ResizeBorder => Borderless ? 0 : 4;

        public Thickness ResizeBorderThickness => new Thickness(ResizeBorder + OuterMarginSize);

        public Thickness InnerContentPadding { get; } = new Thickness(0);

        public int OuterMarginSize
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : _outerMarginSize;
            }
            set
            {
                _outerMarginSize = value;
            }
        }

        public Thickness OuterMarginSizeThickness { get { return new Thickness(OuterMarginSize); } }

        public int WindowRadius
        {
            get
            {
                // If it is maximized or docked, no border
                return Borderless ? 0 : _windowRadius;
            }
            set
            {
                _windowRadius = value;
            }
        }

        public CornerRadius WindowCornerRadius { get { return new CornerRadius(WindowRadius); } }

        public int TitleHeight { get; set; } = 42;

        public GridLength TitleHeightGridLength { get { return new GridLength(TitleHeight + ResizeBorder); } }

        private Point GetMousePosition()
        {
            var position = Mouse.GetPosition(_window);
            // convert to screen coords
            return new Point(position.X + _window.Left, position.Y + _window.Top);
        }

        private void WindowResized()
        {
            // Fire off events for all properties that are affected by a resize
            OnPropertyChanged(nameof(Borderless));
            OnPropertyChanged(nameof(ResizeBorderThickness));
            OnPropertyChanged(nameof(OuterMarginSize));
            OnPropertyChanged(nameof(OuterMarginSizeThickness));
            OnPropertyChanged(nameof(WindowRadius));
            OnPropertyChanged(nameof(WindowCornerRadius));
        }

        #endregion
        
    }
}
