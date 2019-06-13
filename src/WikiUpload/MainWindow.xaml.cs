using System;
using System.Runtime.InteropServices;
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
            this.DataContext = new WindowViewModel(this);
            Navigator.NavigationService = MainFrame.NavigationService;
            this.Loaded += new RoutedEventHandler(Window_Loaded);
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
                var about = new AboutWindow();
                WindowInteropHelper aboutHandle = new WindowInteropHelper(about);
                aboutHandle.Owner = hwnd;
                about.ShowDialog();
            }
            return IntPtr.Zero;
        }
    }
}
