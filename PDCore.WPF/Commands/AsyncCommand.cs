using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PDCore.WPF.Commands
{
    public static class TaskUtilities
    {
        public static IErrorHandler GlobalErrorHandler;

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                (handler ?? GlobalErrorHandler)?.HandleError(ex);

                if (handler == null)
                    throw;
            }
        }
    }

    public interface IAsyncCommand<T> : ICommand
    {
        Task ExecuteAsync(T parameter);
        bool CanExecute(T parameter);
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }

    public class AsyncCommand<T> : IAsyncCommand<T>
    {
        public event EventHandler CanExecuteChanged;

        private bool _isExecuting;
        private readonly Func<T, Task> _execute;
        private readonly Func<T, bool> _canExecute;
        private readonly IErrorHandler _errorHandler;
        private readonly bool suppressIsExecuting;

        public AsyncCommand(Func<T, Task> execute,
            Func<T, bool> canExecute = null,
            IErrorHandler errorHandler = null,
            bool suppressIsExecuting = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
            this.suppressIsExecuting = suppressIsExecuting;
        }

        public bool CanExecute(T parameter)
        {
            return (suppressIsExecuting || !_isExecuting) && (_canExecute?.Invoke(parameter) ?? true);
        }

        public async Task ExecuteAsync(T parameter)
        {
            if (CanExecute(parameter))
            {
                try
                {
                    _isExecuting = true;
                    await _execute(parameter);
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }


        #region Explicit implementations

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync((T)parameter).FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }

    public class AsyncCommand : IAsyncCommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private bool _isExecuting;
        private readonly Func<Task> _execute;
        private readonly Func<bool> _canExecute;
        private readonly IErrorHandler _errorHandler;
        private readonly bool suppressIsExecuting;

        public AsyncCommand(
            Func<Task> execute,
            Func<bool> canExecute = null,
            IErrorHandler errorHandler = null,
            bool suppressIsExecuting = false)
        {
            _execute = execute;
            _canExecute = canExecute;
            _errorHandler = errorHandler;
            this.suppressIsExecuting = suppressIsExecuting;
        }

        public bool CanExecute()
        {
            return (suppressIsExecuting || !_isExecuting) && (_canExecute?.Invoke() ?? true);
        }

        public async Task ExecuteAsync()
        {
            if (CanExecute())
            {
                try
                {
                    _isExecuting = true;
                    await _execute();
                }
                finally
                {
                    _isExecuting = false;
                }
            }

            RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }


        #region Explicit implementations

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAsync().FireAndForgetSafeAsync(_errorHandler);
        }

        #endregion
    }
}
