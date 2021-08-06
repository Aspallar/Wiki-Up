using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WikiUpload
{
    internal class FocusedPopup : Popup
    {
        public FocusedPopup() : base()
        {
            KeyDown += Popup_KeyDown;
            Opened += Popup_Opened;
            Closed += Popup_Closed;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            ExitFocus?.Focus();
        }

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                IsOpen = false;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            var initialFocus = InitialFocus;
            if (initialFocus != null)
            {
                initialFocus.Focus();
                if (initialFocus is TextBox tb)
                    tb.SelectAll();
            }
        }

        #region InitiaFocus property

        public Control InitialFocus
        {
            get { return (Control)GetValue(InitialFocusProperty); }
            set { SetValue(InitialFocusProperty, value); }
        }

        public static readonly DependencyProperty InitialFocusProperty =
            DependencyProperty.Register(
                nameof(InitialFocus),
                typeof(Control),
                typeof(FocusedPopup));

        #endregion

        #region ExitFocus property

        public Control ExitFocus
        {
            get { return (Control)GetValue(ExitFocusProperty); }
            set { SetValue(ExitFocusProperty, value); }
        }

        public static readonly DependencyProperty ExitFocusProperty =
            DependencyProperty.Register(
                nameof(ExitFocus),
                typeof(Control),
                typeof(FocusedPopup));

        #endregion

    }
}
