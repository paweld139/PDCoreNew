using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PDCoreNew.Context.IContext;
using PDCoreNew.Extensions;
using PDCoreNew.Utils;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PDCoreNew.Helpers.Seeder
{
    public abstract class Seeder
    {
        protected readonly IEntityFrameworkCoreDbContext context;

        protected Seeder(IEntityFrameworkCoreDbContext context)
        {
            this.context = context;
        }

        protected virtual string GetCsvFilePath(string fileName) => Path.Combine(SecurityUtils.GetApplicationExeDirectory(), "SampleData", "Csv", "Data", fileName);

        protected virtual Task ExecuteMigrateAsync() => context.Database.MigrateAsync();

        protected virtual async Task<bool> CanMigrate()
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            return pendingMigrations.Any();
        }

        protected virtual async Task MigrateAsync()
        {
            bool canMigrate = await CanMigrate();

            if (canMigrate)
                await ExecuteMigrateAsync();
        }

        protected abstract Task SeedDataAsync();

        public async Task SeedAsync()
        {
            await MigrateAsync();

            await SeedDataAsync();
        }

        protected Task SeedFromCsv<TEntity, TMap>(string fileName) where TEntity : class where TMap : ClassMap<TEntity>, new()
        {
            string path = GetCsvFilePath(fileName);

            return context.AddFromCsv<TEntity, TMap>(path);
        }
    }
}
