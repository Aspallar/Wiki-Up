using System;
using System.Windows;
using System.Windows.Interop;

namespace WikiUpload
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(this);
            CreateApplicationServices();
            Loaded += Window_Loaded;
        }

        private void CreateApplicationServices()
        {
            App.Navigator = new NavigationService(MainFrame.NavigationService);
            App.ServiceLocator = new ServiceLocator();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            NativeMethods.CreateSystemMenu(handle);
            HwndSource source = HwndSource.FromHwnd(handle);
            source.AddHook(new HwndSourceHook(WndProc));
        }

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == NativeMethods.WM_SYSCOMMAND && wParam == NativeMethods.AboutSysMenuId)
            {
                handled = true;
                var about = new AboutBoxWindow();
                WindowInteropHelper aboutHandle = new WindowInteropHelper(about);
                aboutHandle.Owner = hwnd;
                about.ShowDialog();
            }
            return IntPtr.Zero;
        }
    }
}
