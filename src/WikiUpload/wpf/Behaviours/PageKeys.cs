using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;

namespace WikiUpload
{
    internal static class PageKeys
    {
        public static readonly DependencyProperty KeysEnabledProperty =
            DependencyProperty.RegisterAttached
            (
                "KeysEnabled",
                typeof(bool),
                typeof(ListBox),
                new UIPropertyMetadata(false, OnEnabledChanged)
            );

        public static bool GetKeysEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(KeysEnabledProperty);
        }

        public static void SetKeysEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(KeysEnabledProperty, value);
        }

        private static void OnEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is ListBox listBox))
                return;

            if (e.NewValue != null)
                listBox.PreviewKeyDown += ListBox_PreviewKeyDown;
            else
                listBox.PreviewKeyDown -= ListBox_PreviewKeyDown;

        }

        private static void ListBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!(sender is ListBox listBox))
                return;

            if (listBox.SelectedIndex != -1 && Keyboard.IsKeyUp(Key.LeftShift))
            {
                if (e.Key == Key.PageDown)
                {
                    var index = PageDown(listBox);
                    SelectAndFocus(listBox, index);
                    e.Handled = true;
                }
                else if (e.Key == Key.PageUp)
                {
                    var index = PageUp(listBox);
                    SelectAndFocus(listBox, index);
                    e.Handled = true;
                }
            }
        }

        private static int PageUp(ListBox listBox)
        {
            int index = listBox.SelectedIndex - listBox.GetViewedItemCount();
            if (index < 0)
                index = 0;
            return index;
        }

        private static int PageDown(ListBox listBox)
        {
            int index = listBox.SelectedIndex + listBox.GetViewedItemCount();
            if (index >= listBox.Items.Count)
                index = listBox.Items.Count - 1;
            return index;
        }

        private static void SelectAndFocus(ListBox listBox, int index)
        {
            listBox.SelectedIndex = index;
            listBox.FocusSelectedItem();
        }
    }
}
