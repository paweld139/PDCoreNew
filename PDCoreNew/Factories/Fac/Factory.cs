namespace PDCoreNew.Factories.Fac
{
    public abstract class Factory<T>
    {
        public abstract T Create(params object[] parameters);

        public virtual T Get(params object[] parameters)
        {
            var provider = Create(parameters);

            return provider;
        }
    }
}
