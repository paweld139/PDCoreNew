namespace PDCoreNew.Contracts
{
    public interface IFactory<T> where T : class
    {
        T Get(params object[] parameters);
    }
}
