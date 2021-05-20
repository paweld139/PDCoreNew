namespace PDCore.Lazy.Interfaces
{
    public interface IValueHolder<T>
    {
        T GetValue(object parameter);
    }
}
