using System;
using System.Windows.Input;

namespace Gui.Commands
{
    /// <summary>
    /// Represents a command.
    /// </summary>
    public class Command : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        /// <param name="execute">Execution action.</param>
        /// <param name="canExecute">Func that determines whether the command can execute.</param>
        public Command(Action execute, Func<bool> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter = null) => canExecute();

        public void Execute(object parameter = null) => execute();
    }
}
