using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PDCoreNew.Context.IContext;
using PDCoreNew.Extensions;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
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

        private static string CurrentApplicationExeDirectory => SecurityUtils.GetApplicationExeDirectory();


        protected virtual string GetCsvFilePath(string fileName) => Path.Combine(CurrentApplicationExeDirectory, "SampleData", "Csv", "Data", fileName);

        protected virtual string GetJsonFilePath(string fileName) => Path.Combine(CurrentApplicationExeDirectory, "SampleData", "Json", fileName);

        protected virtual string GetTextFilePath(string fileName) => Path.Combine(CurrentApplicationExeDirectory, "SampleData", "Txt", fileName);

        protected virtual string GetHtmlFilePath(string fileName) => Path.Combine(CurrentApplicationExeDirectory, "SampleData", "Html", fileName);

        protected virtual string GetFilePath(string fileName)
        {
            string extension = IOUtils.GetSimpleExtension(fileName);

            string extensionTitleCase = extension.ToTitleCase();

            return Path.Combine(CurrentApplicationExeDirectory, "SampleData", extensionTitleCase, fileName);
        }


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

        protected Task SeedFromJson<TEntity>(string fileName) where TEntity : class
        {
            string path = GetJsonFilePath(fileName);

            return context.AddFromJson<TEntity>(path);
        }

        protected Task SeedFromJson<TEntity, TData>(string fileName, Func<TData, ValueTask<IEnumerable<TEntity>>> func) where TEntity : class
        {
            string path = GetJsonFilePath(fileName);

            return context.AddFromJson(path, func);
        }

        protected Task SeedDictionaryFromJson<TEntity, TElement>(string fileName, Func<string, IEnumerable<TElement>, IEnumerable<TEntity>> func) where TEntity : class
        {
            string path = GetJsonFilePath(fileName);

            return context.AddDictionaryFromJson(path, func);
        }

        protected Task AddFromFile<TEntity>(string fileName, Func<string, TEntity> func, Expression<Func<TEntity, bool>> expression)
            where TEntity : class
        {
            string path = GetFilePath(fileName);

            return context.AddFromFile(path, func, expression);
        }
    }
}
