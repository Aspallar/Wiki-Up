using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WikiUpload
{
    internal class FocusedPopup : Popup
    {
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
                typeof(FocusedPopup),
                new UIPropertyMetadata(new PropertyChangedCallback(OnInitialFocusChanged)));

        private static void OnInitialFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)sender;

            if (e.NewValue != null)
            {
                if (e.OldValue == null)
                {
                    popup.Opened += Popup_Opened;
                    popup.KeyDown += Popup_KeyDown;
                }
            }
            else
            {
                popup.Opened -= Popup_Opened;
                popup.KeyDown -= Popup_KeyDown;
            }
        }

        private static void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                ((Popup)sender).IsOpen = false;
        }

        private static void Popup_Opened(object sender, EventArgs e)
        {
            if (sender is DependencyObject d && d.GetValue(InitialFocusProperty) is Control control)
            {
                control.Focus();
                if (control is TextBox tb)
                    tb.SelectAll();
            }
        }

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
                typeof(FocusedPopup),
                new UIPropertyMetadata(new PropertyChangedCallback(OnExitFocusChanged)));

        private static void OnExitFocusChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var popup = (Popup)sender;

            if (e.NewValue != null)
            {
                if (e.OldValue == null)
                    popup.Closed += Popup_Closed;
            }
            else
            {
                popup.Closed -= Popup_Closed;
            }
        }

        private static void Popup_Closed(object sender, EventArgs e)
        {
            if (sender is DependencyObject d && d.GetValue(ExitFocusProperty) is Control control)
                control.Focus();
        }

        #endregion

    }
}
