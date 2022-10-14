using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    internal static class ListBoxExtensions
    {
        public static void FocusSelectedOrFirstVisibleItem(this ListBox listBox)
        {
            if (listBox.SelectedItem == null)
            {
                var firstVisibleItemIndex = GetFirstVisibleItemIndex(listBox);
                if (firstVisibleItemIndex != -1)
                    listBox.SelectedIndex = firstVisibleItemIndex;
            }

            if (listBox.SelectedItem != null)
                listBox.FocusSelectedItem();
            else
                listBox.Focus();
        }

        public static void FocusSelectedItem(this ListBox listBox)
        {
            var itemContainer = listBox.ScrollToSelectedItem();
            itemContainer.Focus();
        }

        private static int GetFirstVisibleItemIndex(ListBox listBox)
        {
            for (int index = 0; index < listBox.Items.Count; index++)
            {
                if (IsVisibleInUI((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[index]), listBox))
                    return index;
            }
            return -1;
        }

        public static int GetViewedItemCount(this ListBox listBox)
        {
            int index = GetFirstVisibleItemIndex(listBox);

            if (index++ == -1)
                return 0;

            int count = 1;
            while (index < listBox.Items.Count && IsVisibleInUI((ListBoxItem)listBox.ItemContainerGenerator.ContainerFromItem(listBox.Items[index]), listBox))
            {
                ++count;
                ++index;
            }

            return count;
        }

        public static UIElement ScrollToSelectedItem(this ListBox listBox)
        {
            listBox.ScrollIntoView(listBox.SelectedItem);
            var itemContainer = (UIElement)listBox.ItemContainerGenerator.ContainerFromItem(listBox.SelectedItem);
            itemContainer.UpdateLayout();
            return itemContainer;
        }

        // From: https://stackoverflow.com/a/1517794
        private static bool IsVisibleInUI(FrameworkElement element, FrameworkElement container)
        {
            Rect elemenBounds = element.TransformToAncestor(container)
                .TransformBounds(new Rect(
                    0.0,
                    0.0,
                    element.ActualWidth,
                    element.ActualHeight));

            return elemenBounds.Top >= 0 && elemenBounds.Top < container.ActualHeight;
        }

    }
}
