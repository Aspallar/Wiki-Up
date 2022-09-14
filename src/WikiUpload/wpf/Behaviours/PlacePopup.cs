using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WikiUpload
{
    internal static class PlacePopup
    {
        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached
            (
                "Target",
                typeof(Popup),
                typeof(PlacePopup),
                new UIPropertyMetadata(null, OnTargetChanged)
            );

        public static Popup GetTarget(DependencyObject obj)
        {
            return (Popup)obj.GetValue(TargetProperty);
        }

        public static void SetTarget(DependencyObject obj, SelectRange value)
        {
            obj.SetValue(TargetProperty, value);
        }

        private static void OnTargetChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Button button))
                return;

            button.Click -= SynchronizeWithPopup;
            if (e.NewValue != null)
                button.Click += SynchronizeWithPopup;
        }

        private static void SynchronizeWithPopup(object sender, System.EventArgs e)
        {
            if (!(sender is Button button))
                return;

            var popup = GetTarget(button);
            if (popup.StaysOpen)
                popup.IsOpen = false;
            popup.PlacementTarget = button;
        }
    }
}
