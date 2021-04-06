using PDCore.Entities.DTO;
using PDCore.Lazy.Proxies;
using PDCore.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PDCore.Repositories.IRepo
{
    public interface ILogRepo : ISqlRepositoryEntityFramework<LogModel>
    {
        Task<List<LogDetailsProxy>> GetAsync(LogDTO log, CancellationToken cancellationToken);
        Task<List<LogDetailsProxy>> GetAsync(LogDTO log);
    }
}
