using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace WikiUpload
{
    public class PopupClosedFocusProperty : BaseAttachedProperty<PopupClosedFocusProperty, object>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && sender is Popup popup)
                popup.Closed += Popup_Closed;
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (sender is DependencyObject d && d.GetValue(PopupClosedFocusProperty.ValueProperty) is Control control)
                control.Focus();
        }
    }
}
