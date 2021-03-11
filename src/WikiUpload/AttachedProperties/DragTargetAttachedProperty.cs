using System;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public interface IFileDropTarget
    {
        void OnFileDrop(string[] filepaths, bool controlKeyPressed );
    }

    public class DropFileTargetProperty : BaseAttachedProperty<DropFileTargetProperty, object>  { }

    public class IsDropFileEnabledProperty : BaseAttachedProperty<IsDropFileEnabledProperty, bool>
    {
        public override void OnValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue && sender is DependencyObject d && d is Control control)
                control.Drop += OnDrop;
        }

        private static void OnDrop(object sender, DragEventArgs dragEventArgs)
        {
            if (!(sender is DependencyObject d))
                return;

            if (d.GetValue(DropFileTargetProperty.ValueProperty) is IFileDropTarget target)
            {
                var paths = GetDragData(dragEventArgs.Data);
                if (paths != null)
                {
                    var controlKeyPressed = (dragEventArgs.KeyStates & DragDropKeyStates.ControlKey) == DragDropKeyStates.ControlKey;
                    target.OnFileDrop(paths, controlKeyPressed);
                }
            } 
            else
            {
                throw new NotSupportedException($"{nameof(DropFileTargetProperty)} value must implement {nameof(IFileDropTarget)}");
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

