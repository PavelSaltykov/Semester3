using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.Commands
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;

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

        public async Task ExecuteAsync()
        {
            await execute();
        }
    }
}
