using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public class ViewedItemProperty : BaseAttachedProperty<ViewedItemProperty, object> 
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue && sender is DependencyObject d && d is ListBox listBox)
                listBox.ScrollIntoView(e.NewValue);
        }
    }
}
