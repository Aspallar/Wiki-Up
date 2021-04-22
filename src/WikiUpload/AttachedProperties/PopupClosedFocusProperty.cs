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
            if (sender is not Popup popup)
                throw new NotSupportedException($"{nameof(PopupClosedFocusProperty)} may only be attached to a Popup ccontrol");

            if (e.NewValue != null)
            {
                if (e.NewValue is Control)
                {
                    if (e.OldValue == null)
                        popup.Closed += Popup_Closed;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(PopupClosedFocusProperty)} value must be a control");
                }
            }
            else
            {
                popup.Closed -= Popup_Closed;
            }
        }

        private void Popup_Closed(object sender, EventArgs e)
        {
            if (sender is DependencyObject d && d.GetValue(PopupClosedFocusProperty.ValueProperty) is Control control)
                control.Focus();
        }
    }
}
