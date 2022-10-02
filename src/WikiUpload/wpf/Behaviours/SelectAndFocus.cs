using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    internal static class SelectAndFocus
    {
        public static readonly DependencyProperty SelectionProperty =
            DependencyProperty.RegisterAttached
            (
                "Selection",
                typeof(SelectRange),
                typeof(SelectAndFocus),
                new UIPropertyMetadata(null, OnSelectionChanged)
            );

        public static SelectRange GetSelection(DependencyObject obj)
        {
            return (SelectRange)obj.GetValue(SelectionProperty);
        }

        public static void SetSelection(DependencyObject obj, SelectRange value)
        {       
            obj.SetValue(SelectionProperty, value);
        }

        private static void OnSelectionChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is TextBox textBox))
                return;

            if (e.NewValue != null)
            {
                var selection = (SelectRange)e.NewValue;
                textBox.Select(selection.Start, selection.Length);
                textBox.ScrollToEnd();
                textBox.Focus();
            }
        }
    }

}
