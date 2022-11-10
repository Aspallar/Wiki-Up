using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class WikiUpWindow : Window
    {
        public string WindowPlacementSettingsPrefix
        {
            get { return (string)GetValue(WindowPlacementSettingsPrefixProperty); }
            set { SetValue(WindowPlacementSettingsPrefixProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WindowPlacementSettingsName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WindowPlacementSettingsPrefixProperty =
            DependencyProperty.Register(nameof(WindowPlacementSettingsPrefix), typeof(string), typeof(WikiUpWindow), new PropertyMetadata(null));

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            RestoreWindowPlacement();
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            SaveWindowPlacement();
        }

         private void SaveWindowPlacement()
        {
            var prefix = WindowPlacementSettingsPrefix;

            if (Settings.Default.IsWindowPlacementEnabled(prefix))
            {
                WindowPlacement wp;
                var hwnd = new WindowInteropHelper(this).Handle;
                NativeMethods.GetWindowPlacement(hwnd, out wp);
                Settings.Default.SetWindowPlacement(prefix, wp);
                Settings.Default.Save();
            }
        }

        private void RestoreWindowPlacement()
        {
            try
            {
                var prefix = WindowPlacementSettingsPrefix;
                if (Settings.Default.IsWindowPlacementEnabled(prefix))
                {
                    var wp = Settings.Default.GetWindowPlacement(prefix);
                    if (wp.length != 0)
                    {
                        wp.length = Marshal.SizeOf(typeof(WindowPlacement));
                        wp.flags = 0;
                        wp.showCmd = (wp.showCmd == NativeMethods.SwShowminimized ? NativeMethods.SwShownormal : wp.showCmd);
                        var hwnd = new WindowInteropHelper(this).Handle;
                        NativeMethods.SetWindowPlacement(hwnd, ref wp);
                    }
                }
            }
            catch
            {
                // ignored
#if DEBUG
                throw;
#endif
            }
        }

    }
}
