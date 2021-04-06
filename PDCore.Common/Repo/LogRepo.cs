using AutoMapper;
using PDCore.Common.Context.IContext;
using PDCore.Entities.DTO;
using PDCore.Extensions;
using PDCore.Interfaces;
using PDCore.Lazy.Proxies;
using PDCore.Models;
using PDCore.Repositories.IRepo;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PDCore.Common.Repositories.Repo
{
    public sealed class LogRepo : SqlRepositoryEntityFrameworkAsync<LogModel>, ILogRepo
    {
        public LogRepo(IEntityFrameworkDbContext ctx, ILogger logger, IMapper mapper) : base(ctx, logger, mapper)
        {
        }

        public LogRepo(IEntityFrameworkDbContext ctx) : base(ctx, null, null)
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
