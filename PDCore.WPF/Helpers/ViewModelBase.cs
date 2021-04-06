using PDCore.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PDCore.WPF.Helpers
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<bool> LoadingChanged;

        public event EventHandler TaskSuccessfullyCompleted;


        private bool isInitialized;

        public bool SuppressIsInitialized;
        private readonly ILogger logger;

        protected ViewModelBase(ILogger logger)
        {
            OnInitialize();
            this.logger = logger;
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnLoadingChanged(bool isLoading) => LoadingChanged?.Invoke(this, isLoading);

        protected virtual void OnTaskSuccessfullyCompleted() => TaskSuccessfullyCompleted?.Invoke(this, EventArgs.Empty);

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;

            OnPropertyChanged(propertyName);

            return true;
        }

        protected abstract void OnInitialize();

        protected abstract Task Refresh();

        public Task Initialize()
        {
            var result = Task.CompletedTask;

            if (SuppressIsInitialized || !isInitialized)
            {
                result = Refresh();

                isInitialized = true;
            }

            return result;
        }

        protected async Task Execute(Func<Task> func)
        {
            try
            {
                OnLoadingChanged(true);

                await func();

                OnTaskSuccessfullyCompleted();
            }
            finally
            {
                OnLoadingChanged(false);
            }
        }

        protected void Execute(Action action)
        {
            Execute(() =>
            {
                action();

                return Task.CompletedTask;
            }).Wait();
        }
    }
}
