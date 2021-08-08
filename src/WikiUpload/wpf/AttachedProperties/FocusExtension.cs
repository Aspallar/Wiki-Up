using System.Windows;

namespace WikiUpload
{
    public static class FocusExtension
    {
        public static bool GetIsFocused(DependencyObject obj)
            => (bool)obj.GetValue(IsFocusedProperty);

        public static void SetIsFocused(DependencyObject obj, bool value)
            => obj.SetValue(IsFocusedProperty, value);

        public static readonly DependencyProperty IsFocusedProperty =
            DependencyProperty.RegisterAttached(
                "IsFocused",
                typeof(bool),
                typeof(FocusExtension),
                new UIPropertyMetadata(false, OnIsFocusedPropertyChanged));

        private static void OnIsFocusedPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
                ((UIElement)d).Focus();
        }
    }
}
