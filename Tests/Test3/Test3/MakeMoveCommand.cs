using System;
using System.Windows.Input;

namespace Test3
{
    public class MakeMoveCommand : ICommand
    {
        private readonly Action<int> execute;
        private readonly Func<int, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public MakeMoveCommand(Action<int> execute, Func<int, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null || canExecute(int.Parse(parameter.ToString()));
        }

        public void Execute(object parameter) => execute(int.Parse(parameter.ToString()));
    }
}
