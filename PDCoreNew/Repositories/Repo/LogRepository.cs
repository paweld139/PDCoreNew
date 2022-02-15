using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PDCoreNew.Context.IContext;
using PDCoreNew.Entities.DTO;
using PDCoreNew.Extensions;
using PDCoreNew.Lazy.Proxies;
using PDCoreNew.Models;
using PDCoreNew.Repositories.IRepo;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDCoreNew.Repositories.Repo
{
    public class LogRepository : SqlRepositoryEntityFrameworkCore<LogModel>, ILogRepository
    {
        public LogRepository(IEntityFrameworkCoreDbContext ctx, ILogger<LogModel> logger, IMapper mapper)
            : base(ctx, logger, mapper, null)
        {
        }

        public Task<List<LogDetailsProxy>> GetAsync(LogDTO log, CancellationToken cancellationToken)
        {
            return mapper.ProjectTo<LogDetailsProxy>(
                    Find(l => log.LogType == null || l.LogLevel == log.LogType.Value)
                    .AsNoTracking()
                    .FindByDateCreated(log.DateCreated.ToUniversalTime(), log.DateCreatedTo.ToUniversalTime())
                    .OrderByDescending(l => l.DateCreated)
                    ).ToListAsync(cancellationToken);
        }

        public Task<List<LogDetailsProxy>> GetAsync(LogDTO log) => GetAsync(log, CancellationToken.None);
    }
}
