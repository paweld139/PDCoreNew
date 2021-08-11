using System;
using System.Threading.Tasks;

namespace PDCoreNew.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();
        Task CommitAsync();

        void CleanUp();

        void CleanUp<TEntity>() where TEntity : class;
    }
}
