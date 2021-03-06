using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WikiUpload
{
    public class PopupInitialFocusProperty : BaseAttachedProperty<PopupInitialFocusProperty, object>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && sender is Popup popup)
                popup.Opened += Popup_Opened;
        }

        private void Popup_Opened(object sender, EventArgs e)
        {
            if (sender is DependencyObject d)
            {
                if (d.GetValue(PopupInitialFocusProperty.ValueProperty) is Control control)
                {
                    control.Focus();
                    if (control is TextBox tb)
                        tb.SelectAll();
                }
            }
        }
    }
}
