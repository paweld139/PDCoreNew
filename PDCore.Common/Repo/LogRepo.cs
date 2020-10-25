using PDCore.Common.Context.IContext;
using PDCore.Models;
using PDCore.Repositories.IRepo;

namespace PDCore.Common.Repositories.Repo
{
    public sealed class LogRepo : SqlRepositoryEntityFrameworkAsync<LogModel>, ILogRepo
    {
        public LogRepo(IEntityFrameworkDbContext ctx) : base(ctx, null, null)
        {

        }
    }
}
