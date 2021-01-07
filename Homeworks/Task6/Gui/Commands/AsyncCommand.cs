using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.Commands
{
    /// <summary>
    /// Represents an async command.
    /// </summary>
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCommand"/> class.
        /// </summary>
        /// <param name="executeAsync">Async execution func.</param>
        /// <param name="canExecute">Func that determines whether the command can execute.</param>
        public AsyncCommand(Func<Task> executeAsync, Func<bool> canExecute)
        {
            execute = executeAsync;
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter = null) => canExecute();

        public async void Execute(object parameter = null) => await ExecuteAsync();

        /// <summary>
        /// Defines the async execution method.
        /// </summary>
        /// <returns></returns>
        public async Task ExecuteAsync() => await execute();
    }
}
