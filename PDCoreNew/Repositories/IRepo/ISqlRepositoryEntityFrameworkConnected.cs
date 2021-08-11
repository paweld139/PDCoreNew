using PDCoreNew.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface ISqlRepositoryEntityFrameworkConnected<T> : ISqlRepositoryEntityFrameworkAsync<T> where T : class, IModificationHistory, new()
    {
        ObservableCollection<T> GetAllFromMemory();

        Task<ObservableCollection<T>> GetAllFromMemoryAsync();


        T Add();
        T AddAndReturn(T entity);
    }
}
