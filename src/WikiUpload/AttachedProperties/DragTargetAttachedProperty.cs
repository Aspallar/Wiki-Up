using System;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public interface IFileDropTarget
    {
        void OnFileDrop(string[] filepaths, bool controlKeyPressed );
    }

    public class DropFileTargetProperty : BaseAttachedProperty<DropFileTargetProperty, object>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not Control control)
                throw new NotSupportedException($"{nameof(DropFileTargetProperty)} may only be attached to controls");

            if (e.NewValue != null)
            {
                if (e.NewValue is IFileDropTarget)
                {
                    if (e.OldValue == null)
                        control.Drop += OnDrop;
                }
                else
                {
                    throw new NotSupportedException($"{nameof(DropFileTargetProperty)} value must implement {nameof(IFileDropTarget)}");
                }
            }
            else
            {
                control.Drop -= OnDrop;
            }

        }

        private static void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (sender is not DependencyObject d)
                return;

            var target = (IFileDropTarget)d.GetValue(DropFileTargetProperty.ValueProperty);
            if (target != null)
            {
                var paths = GetDragData(dragEventArgs.Data);
                if (paths != null)
                {
                    var controlKeyPressed = (dragEventArgs.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey;
                    target.OnFileDrop(paths, controlKeyPressed);
                }
            }
        }

        private static string[] GetDragData(IDataObject data)
        {
            if (data.GetDataPresent(DataFormats.FileDrop))
                return (string[])data.GetData(DataFormats.FileDrop);
            else if (data.GetDataPresent(DataFormats.Text))
                return new string[] { (string)data.GetData(DataFormats.Text) };
            else
                return null;
        }
    }
}

