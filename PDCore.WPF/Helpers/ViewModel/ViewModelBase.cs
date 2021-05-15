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

        public event EventHandler TaskCompleted;


        private bool isInitialized;
        private bool isLoading;

        public bool SuppressIsInitialized;

        public bool IsLoading { get => isLoading; private set => SetProperty(ref isLoading, value); }

        protected ViewModelBase()
        {
            OnInitialize();
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnLoadingChanged(bool isLoading, bool executeOnLoadingChanged)
        {
            IsLoading = isLoading;

            if (executeOnLoadingChanged)
                LoadingChanged?.Invoke(this, isLoading);
        }

        protected virtual void OnTaskCompleted() => TaskCompleted?.Invoke(this, EventArgs.Empty);

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

        protected async Task Execute(Func<Task> func, bool executeOnLoadingChanged = true, bool executeOnTaskCompleted = true)
        {
            try
            {
                OnLoadingChanged(true, executeOnLoadingChanged);

                await func();
            }
            finally
            {
                OnLoadingChanged(false, executeOnLoadingChanged);

                if (executeOnTaskCompleted)
                    OnTaskCompleted();
            }
        }

        protected void Execute(Action action, bool executeOnLoadingChanged = true, bool executeOnTaskCompleted = true)
        {
            Execute(() =>
            {
                action();

                return Task.CompletedTask;
            }, executeOnLoadingChanged, executeOnTaskCompleted).Wait();
        }
    }
}
