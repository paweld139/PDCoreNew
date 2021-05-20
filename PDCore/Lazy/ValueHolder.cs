using PDCore.Lazy.Interfaces;
using System;

namespace PDCore.Lazy
{
    public class ValueHolder<T> : IValueHolder<T>
    {
        private readonly Func<object, T> getValue;
        private T value;

        public ValueHolder(Func<object, T> getValue)
        {
            this.getValue = getValue;
        }

        public T GetValue(object parameter)
        {
            if (value == null)
            {
                value = getValue(parameter);
            }

            return value;
        }
    }
}
