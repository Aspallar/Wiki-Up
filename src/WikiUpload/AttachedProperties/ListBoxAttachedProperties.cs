using System;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public class ViewedItemProperty : BaseAttachedProperty<ViewedItemProperty, object> 
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && e.NewValue != e.OldValue)
            {
                if (sender is ListBox listBox)
                {
                    listBox.ScrollIntoView(e.NewValue);
                }
                else
                {
                    var inbalidTypeName = sender.GetType().Name;
                    throw new NotSupportedException($"{nameof(ViewedItemProperty)} may only be attached to a ListBox not a {inbalidTypeName}");
                }
            }
        }
    }
}
