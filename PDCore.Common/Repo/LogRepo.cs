
using AutoMapper;
using PDCore.Common.Context.IContext;
using PDCore.Entities.DTO;
using PDCore.Lazy.Proxies;
using PDCore.Models;
using PDCore.Repositories.IRepo;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using PDCore.Extensions;
using System.Linq;

namespace PDCore.Common.Repositories.Repo
{
    public sealed class LogRepo : SqlRepositoryEntityFrameworkAsync<LogModel>, ILogRepo
    {
        public LogRepo(IEntityFrameworkDbContext ctx, IMapper mapper) : base(ctx, null, mapper)
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
