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
            if (sender is not Popup popup)
                throw new NotSupportedException($"{nameof(PopupInitialFocusProperty)} may only be attached to a Popup ccontroll");

            if (e.NewValue != null)
            {
                if (e.NewValue is Control)
                {
                    if (e.OldValue == null)
                        popup.Opened += Popup_Opened;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(PopupInitialFocusProperty)} value must be a control");
                }
            }
            else
            {
                popup.Opened -= Popup_Opened;
            }
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
