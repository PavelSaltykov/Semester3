﻿using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Gui.Commands
{
    public abstract class AsyncCommandBase : ICommand
    {
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter) => await ExecuteAsync(parameter);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}
