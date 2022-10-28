using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WikiUpload
{
    internal class DeleteListBox : ListBox
    {
        public DeleteListBox() : base() { }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
                ExecuteDeleteSelectedCommand();
            base.OnKeyDown(e);
        }

        private void ExecuteDeleteSelectedCommand()
        {
            var previousSelectedIndex = SelectedIndex;
            DeleteSelectedCommand?.Execute(SelectedItems);
            if (previousSelectedIndex < Items.Count)
                SelectedIndex = previousSelectedIndex;
            this.FocusSelectedOrFirstVisibleItem();
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
