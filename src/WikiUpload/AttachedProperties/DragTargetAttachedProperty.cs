using System;
using System.Windows;
using System.Windows.Controls;

namespace WikiUpload
{
    public interface IFileDropTarget
    {
        void OnFileDrop(string[] filepaths);
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
                string[] paths = null;
                if (dragEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                    paths = (string[])dragEventArgs.Data.GetData(DataFormats.FileDrop);
                else if (dragEventArgs.Data.GetDataPresent(DataFormats.Text))
                    paths = new string[] { (string)dragEventArgs.Data.GetData(DataFormats.Text) };
                if (paths != null)
                    target.OnFileDrop(paths);
            }
            else
            {
                throw new NotSupportedException($"{nameof(DropFileTargetProperty)} value must implement {nameof(IFileDropTarget)}");
            }
        }
    }
}

