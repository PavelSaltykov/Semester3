﻿using System;
using System.Threading.Tasks;

namespace Gui.Commands
{
    public class ConnectCommand : AsyncCommandBase
    {
        private readonly Func<Task> execute;
        private readonly Func<bool> canExecute;
        private bool isExecuting;

        public ConnectCommand(Func<Task> executeAsync, Func<bool> canExecute)
        {
            execute = executeAsync;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object parameter = null) => !isExecuting && canExecute();

        public override async Task ExecuteAsync(object parameter = null)
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;
                await execute();
            }
            finally
            {
                isExecuting = false;
            }
        }
    }
}