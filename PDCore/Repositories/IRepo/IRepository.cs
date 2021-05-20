namespace PDCore.Repositories.IRepo
{
    public interface IRepository<T> : IReadOnlyRepository<T>, IWriteOnlyRepository<T>
    {

    }
}
