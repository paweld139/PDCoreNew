using PDCoreNew.Context.IContext;
using PDCoreNew.Extensions;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers.Seeder
{
    public abstract class PostgresSeeder : Seeder
    {
        protected PostgresSeeder(IEntityFrameworkCoreDbContext context) : base(context)
        {
        }

        protected override async Task ExecuteMigrateAsync()
        {
            await base.ExecuteMigrateAsync();

            await context.ReloadTypes();
        }
    }
}
