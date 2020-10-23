using System;
using System.Windows.Input;

namespace WikiUpload
{
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The action to run
        /// </summary>
        private readonly Action _action;
        private ICommand cancelSettingsCommand;

        /// <summary>
        /// The event thats fired when the <see cref="CanExecute(object)"/> value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged = (sender, e) => { };


        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action action)
        {
            _action = action;
        }

        public RelayCommand(ICommand cancelSettingsCommand)
        {
            this.cancelSettingsCommand = cancelSettingsCommand;
        }

        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
