namespace PDCoreNew.Repositories.IRepo
{
    public interface IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>
    {

    }
}
