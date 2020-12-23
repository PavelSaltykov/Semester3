using System;
using System.Windows.Input;

namespace Gui.Commands
{
    public class Command : ICommand
    {
        private readonly Action execute;
        private readonly Func<bool> canExecute;

        public Command(Action execute, Func<bool> canExecute=null)
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
