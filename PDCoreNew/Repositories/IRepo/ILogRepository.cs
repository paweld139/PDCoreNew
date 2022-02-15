using PDCoreNew.Entities.DTO;
using PDCoreNew.Lazy.Proxies;
using PDCoreNew.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.IRepo
{
    public interface ILogRepository : ISqlRepositoryEntityFrameworkAsync<LogModel>
    {
        Task<List<LogDetailsProxy>> GetAsync(LogDTO log, CancellationToken cancellationToken);
        Task<List<LogDetailsProxy>> GetAsync(LogDTO log);
    }
}
