﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace PDCore.WPF.Helpers.WPF
{
    public abstract class DialogViewModelBase : ViewModelBase
    {
        private bool? _dialogResult;

        public event EventHandler Closing;

        public string Title { get; private set; }
        public ObservableCollection<Button> DialogButtons { get; }

        public bool? DialogResult
        {
            get { return _dialogResult; }
            set { SetProperty(ref _dialogResult, value); }
        }

        public void Close()
        {
            Closing?.Invoke(this, EventArgs.Empty);
        }
    }
}
