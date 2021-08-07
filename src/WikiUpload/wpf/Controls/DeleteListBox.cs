using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    internal class DeleteListBox : ListBox
    {
        public DeleteListBox() : base()
        {
            KeyDown += DeleteListBox_KeyDown;
        }

        private void DeleteListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                DeleteSelectedCommand?.Execute(SelectedItems);
        }

        public ICommand DeleteSelectedCommand
        {
            get { return (ICommand)GetValue(DeleteSelectedCommandProperty); }
            set { SetValue(DeleteSelectedCommandProperty, value); }
        }

        public static readonly DependencyProperty DeleteSelectedCommandProperty =
            DependencyProperty.Register(
                nameof(DeleteSelectedCommand),
                typeof(ICommand),
                typeof(DeleteListBox));
    }
}
