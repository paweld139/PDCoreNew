namespace PDCore.Builders
{
    public class Builder<T> : IBuilder<T>
    {
        protected T _object;

        public T Build()
        {
            return _object;
        }
    }
}
