using System;
using System.Threading.Tasks;

namespace Gui.Commands
{
    public class ListItemCommand : AsyncCommandBase
    {
        private readonly Func<int, Task> execute;
        private readonly Func<int, bool> canExecute;
        private bool isExecuting;

        public ListItemCommand(Func<int,Task> executeAsync, Func<int,bool> canExecute)
        {
            execute = executeAsync;
            this.canExecute = canExecute;
        }

        public override bool CanExecute(object index = null) => !isExecuting && canExecute((int)index);

        public override async Task ExecuteAsync(object index)
        {
            if (isExecuting)
                return;

            try
            {
                isExecuting = true;
                await execute((int)index);
            }
            finally
            {
                isExecuting = false;
            }
        }
    }
}
