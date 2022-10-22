using System;
using System.Text.RegularExpressions;
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
            Loaded += Popup_Loaded;
            Unloaded += Popup_Unloaded; ;
        }

        private void Popup_Loaded(object sender, RoutedEventArgs e)
        {
            var closeButton = CloseButton;
            if (closeButton != null)
                closeButton.Click += CloseButton_Click;
        }

        private void Popup_Unloaded(object sender, RoutedEventArgs e)
        {
            KeyDown -= Popup_KeyDown;
            Loaded -= Popup_Loaded;
            Unloaded -= Popup_Unloaded;

            var closeButton = CloseButton;
            if (closeButton != null)
                closeButton.Click -= CloseButton_Click;
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            SetInitialFocus();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            SetExitFocus();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            IsOpen = false;
        }

        private void Popup_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape && CloseOnEscape)
                IsOpen = false;
        }

        private void SetInitialFocus()
        {
            var initialFocus = InitialFocus;
            if (initialFocus != null)
                SetInitialFocus(initialFocus);
        }

        private void SetInitialFocus(UIElement initialFocus)
        {
            initialFocus.Focus();
            if (initialFocus is TextBox tb)
                SelectText(tb);
        }

        private void SelectText(TextBox textBox)
        {
            var selectPattern = SelectPattern;
            if (string.IsNullOrEmpty(selectPattern))
                textBox.SelectAll();
            else
                SelectTextUsingPattern(textBox, selectPattern);
        }

        private static void SelectTextUsingPattern(TextBox textBox, string selectPattern)
        {
            var match = Regex.Match(textBox.Text, selectPattern);
            if (match.Success)
            {
                var group = (match.Groups.Count > 1) ? match.Groups[1] : match.Groups[0];
                textBox.Select(group.Index, group.Length);
            }
            else
            {
                textBox.SelectAll();
            }
        }

        private void SetExitFocus() => ExitFocus?.Focus();

        #region InitiaFocus property

        public UIElement InitialFocus
        {
            get { return (UIElement)GetValue(InitialFocusProperty); }
            set { SetValue(InitialFocusProperty, value); }
        }

        public static readonly DependencyProperty InitialFocusProperty =
            DependencyProperty.Register(
                nameof(InitialFocus),
                typeof(UIElement),
                typeof(FocusedPopup));

        #endregion

        #region ExitFocus property

        public UIElement ExitFocus
        {
            get { return (UIElement)GetValue(ExitFocusProperty); }
            set { SetValue(ExitFocusProperty, value); }
        }

        public static readonly DependencyProperty ExitFocusProperty =
            DependencyProperty.Register(
                nameof(ExitFocus),
                typeof(UIElement),
                typeof(FocusedPopup));

        #endregion

        #region CloseButton property

        public Button CloseButton
        {
            get { return (Button)GetValue(CloseButtonProperty); }
            set { SetValue(CloseButtonProperty, value); }
        }

        public static readonly DependencyProperty CloseButtonProperty =
            DependencyProperty.Register(
                nameof(CloseButton),
                typeof(Button),
                typeof(FocusedPopup));

        #endregion

        #region SelectPattern property

        public string SelectPattern
        {
            get { return (string)GetValue(SelectPatternProperty); }
            set { SetValue(SelectPatternProperty, value); }
        }

        public static readonly DependencyProperty SelectPatternProperty =
            DependencyProperty.Register(
                nameof(SelectPattern),
                typeof(string),
                typeof(FocusedPopup));

        #endregion

        #region CloseOnEscape property

        public bool CloseOnEscape
        {
            get { return (bool)GetValue(CloseOnEscapeProperty); }
            set { SetValue(CloseOnEscapeProperty, value); }
        }

        public static readonly DependencyProperty CloseOnEscapeProperty =
            DependencyProperty.Register(
                nameof(CloseOnEscape),
                typeof(bool),
                typeof(FocusedPopup),
                new PropertyMetadata(true));

        #endregion

    }
}
