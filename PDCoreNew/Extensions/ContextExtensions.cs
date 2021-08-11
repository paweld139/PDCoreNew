using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using PDCoreNew.Context.IContext;
using PDCoreNew.Enums;
using PDCoreNew.Interfaces;
using PDCoreNew.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PDCoreNew.Extensions
{
    public static class ContextExtensions
    {
        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            return query.ToQueryString();
        }

        public static DataTable DataTable(this IEntityFrameworkCoreDbContext context, string query)
        {
            return SqlUtils.GetDataTable(query, context.Database.GetDbConnection());
        }

        private static void BeforeSaveChangesWithHistory(IEntityFrameworkCoreDbContext dbContext)
        {
            DateTime dateTime = DateTime.UtcNow;

            IModificationHistory entity;
            EntityState entityState;

            foreach (var history in dbContext.ChangeTracker.Entries()
                            .Where(e => e.Entity is IModificationHistory && (e.State == EntityState.Added || e.State == EntityState.Modified))
                            .Select(e => new { Entity = e.Entity as IModificationHistory, e.State }))
            {
                entity = history.Entity;
                entityState = history.State;

                if (entityState == EntityState.Modified && dbContext.Entry(entity).Metadata.FindProperty("DateCreated") != null)
                    dbContext.Entry(entity).Property(e => e.DateCreated).IsModified = false;

                if (entityState == EntityState.Added)
                    entity.DateCreated = dateTime;

                entity.DateModified = dateTime;
            }
        }

        private static void AfterSaveChangesWithHistory(IEntityFrameworkCoreDbContext dbContext)
        {
            foreach (var history in dbContext.ChangeTracker.Entries()
                                    .Where(e => e.Entity is IModificationHistory)
                                    .Select(e => e.Entity as IModificationHistory))
            {
                history.IsDirty = false;
            }
        }

        public static int SaveChangesWithModificationHistory(this IEntityFrameworkCoreDbContext dbContext)
        {
            BeforeSaveChangesWithHistory(dbContext);

            int result = dbContext.SaveChanges();

            AfterSaveChangesWithHistory(dbContext);

            return result;
        }

        public static async Task<int> SaveChangesWithModificationHistoryAsync(this IEntityFrameworkCoreDbContext dbContext)
        {
            BeforeSaveChangesWithHistory(dbContext);

            int result = await dbContext.SaveChangesAsync();

            AfterSaveChangesWithHistory(dbContext);

            return result;
        }

        public static bool ExistsLocal<T>(this IEntityFrameworkCoreDbContext dbContext, Func<T, bool> predicate) where T : class
        {
            return dbContext.Set<T>().Local.Any(predicate);
        }

        public static T FirstLocal<T>(this IEntityFrameworkCoreDbContext dbContext, Func<T, bool> predicate) where T : class
        {
            return dbContext.Set<T>().Local.First(predicate);
        }

        public static T FirstOrDefautLocal<T>(this IEntityFrameworkCoreDbContext dbContext, Func<T, bool> predicate) where T : class
        {
            return dbContext.Set<T>().Local.FirstOrDefault(predicate);
        }

        public static bool ExistsLocal<T>(this IEntityFrameworkCoreDbContext dbContext, T entity) where T : class
        {
            return dbContext.Set<T>().Local.Contains(entity);
        }

        public static void ConfigureForModificationHistory(this ModelBuilder modelBuilder)
        {
            modelBuilder.Model.GetEntityTypes().ForEach(e =>
            {
                var entity = modelBuilder.Entity(e.ClrType);

                entity.Ignore(nameof(IModificationHistory.IsDirty));

                if (entity.Metadata.FindProperty(nameof(IModificationHistory.DateCreated)) != null)
                    entity.Property(nameof(IModificationHistory.DateCreated))?.HasColumnName("date_created");

                if (entity.Metadata.FindProperty(nameof(IModificationHistory.DateModified)) != null)
                    entity.Property(nameof(IModificationHistory.DateModified))?.HasColumnName("date_modified");
            });
        }

        public static void HandleExceptionOnEdit<T>(this DbUpdateConcurrencyException exception, T entity, Action<string, string> writeError) where T : class, IModificationHistory
        {
            var entry = exception.Entries.Single();

            var clientEntry = entry.CurrentValues;

            var databaseEntry = entry.GetDatabaseValues();

            if (databaseEntry == null)
            {
                writeError(string.Empty, "Unable to save changes. The object was deleted by another user.");
            }
            else
            {
                var databaseValues = (T)databaseEntry.ToObject();

                string error;

                foreach (var property in clientEntry.Properties)
                {
                    if (property.Name.ValueIn(nameof(databaseValues.DateModified), nameof(databaseValues.DateCreated)))
                        continue;

                    var databaseValue = databaseEntry[property];

                    var clientValue = clientEntry[property];

                    if (clientValue != databaseValue && !(clientValue?.Equals(databaseValue) ?? false))
                    {
                        error = "Current value: ";

                        if (property.Name == nameof(databaseValues.RowVersion))
                        {
                            error += Convert.ToBase64String(databaseValues.RowVersion);
                        }
                        else
                        {
                            error += databaseValue;
                        }

                        writeError("model." + property, error);
                    }
                }

                writeError("", "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, "
                    + "save again.");

                entity.RowVersion = databaseValues.RowVersion;
            }
        }

        public static void EditEntity<TEntity>(this DbContext context,
            TEntity entity,
            TypeOfEditEntityProperty typeOfEditEntityProperty,
            params string[] properties) where TEntity : class
        {
            var find = context.Set<TEntity>().Find(entity.GetType().GetProperty("Id").GetValue(entity, null));

            if (find == null)
                throw new Exception("id not found in database");

            if (typeOfEditEntityProperty == TypeOfEditEntityProperty.Ignore)
            {
                foreach (var item in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                {
                    if (!item.CanRead || !item.CanWrite)
                        continue;
                    if (properties.Contains(item.Name))
                        continue;

                    item.SetValue(find, item.GetValue(entity, null), null);
                }
            }
            else if (typeOfEditEntityProperty == TypeOfEditEntityProperty.Take)
            {
                foreach (var item in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                {
                    if (!item.CanRead || !item.CanWrite)
                        continue;
                    if (!properties.Contains(item.Name))
                        continue;

                    item.SetValue(find, item.GetValue(entity, null), null);
                }
            }
            else
            {
                foreach (var item in entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
                {
                    if (!item.CanRead || !item.CanWrite)
                        continue;

                    item.SetValue(find, item.GetValue(entity, null), null);
                }
            }

            context.SaveChanges();
        }

        public static Task AddSimpleDictionary<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context) where TEnum : struct where TEntity : class, ISimpleDictionary, new()
        {
            return context.Add<TEnum, TEntity>(c =>
                               new TEntity
                               {
                                   Id = c.CastObject<int>(),
                                   Name = c.GetDescription()
                               });
        }

        public static Task Add<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context, Func<TEnum, TEntity> func) where TEnum : struct where TEntity : class, IEntity<int>
        {
            var positions = EnumUtils.GetEnumValues<TEnum>().Select(func);

            return Add(context, positions);
        }

        public static Task Add<TEntity>(this IEntityFrameworkCoreDbContext context, IEnumerable<TEntity> positions) where TEntity : class, IEntity<int>
        {
            Task task = Task.CompletedTask;

            var positionsInDatabase = context.Set<TEntity>().Select(c => c.Id);

            var positionsInDatabaseIds = new HashSet<int>(positionsInDatabase);

            var positionsToSave = positions.Where(c => !positionsInDatabaseIds.Contains(c.Id)).ToArray();

            if (positionsToSave.Any())
            {
                task = context.Set<TEntity>().AddRangeAsync(positionsToSave);
            }

            return task;
        }

        public static Task SaveChangesIfExistsAsync(this IEntityFrameworkCoreDbContext context)
        {
            Task task = Task.CompletedTask;

            if (context.ChangeTracker.HasChanges())
                task = context.SaveChangesWithModificationHistoryAsync();

            return task;
        }

        public static async Task ReloadTypes(this IEntityFrameworkCoreDbContext context)
        {
            var connection = (NpgsqlConnection)context.Database.GetDbConnection();

            await connection.OpenConnectionIfClosedAsync();

            connection.ReloadTypes();

            await connection.CloseAsync();
        }

        public static async Task<TEntity> AddOrUpdate<TEntity>(this IEntityFrameworkCoreDbContext context, TEntity entity) where TEntity : class
        {
            var entityEntry = context.Entry(entity);

            var primaryKeyProperies = entityEntry.Context.Model.FindEntityType(typeof(TEntity)).FindPrimaryKey().Properties.ToArray(p => p.Name);

            var t = typeof(TEntity);

            if (!primaryKeyProperies.Any())
            {
                throw new Exception($"{t.FullName} does not have a primary key specified. Unable to exec AddOrUpdate call.");
            }

            var primaryKeyValues = primaryKeyProperies.ToArray(p => entity.GetPropertyValue(p));

            var dbVal = await context.Set<TEntity>().FindAsync(primaryKeyValues);

            if (dbVal != null)
            {
                context.Entry(dbVal).CurrentValues.SetValues(entity);
                context.Set<TEntity>().Update(dbVal);

                entity = dbVal;
            }
            else
            {
                context.Set<TEntity>().Add(entity);
            }

            return entity;
        }

        public static async Task UpdateRange<TEntity>(this IEntityFrameworkCoreDbContext context, IEnumerable<TEntity> entities, bool loadAll = true) where TEntity : class
        {
            if (loadAll)
                await context.Set<TEntity>().LoadAsync();

            await entities.ForEachAsync(e => context.AddOrUpdate(e));
        }

        public static Task AddOrUpdateDictionary<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context, Func<TEnum, TEntity> func) where TEnum : struct where TEntity : class
        {
            var positions = EnumUtils.GetEnumValues<TEnum>().Select(func);

            return context.UpdateRange(positions);
        }

        public static Task AddOrUpdateSimpleType<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context) where TEnum : struct where TEntity : class, ISimpleType, new()
        {
            return context.AddOrUpdateDictionary<TEnum, TEntity>(c =>
                              new TEntity
                              {
                                  Id = c.CastObject<int>(),
                                  Type = c.GetDescription(),
                              });
        }

        public static Task AddOrUpdateDictionaryWithTag<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context)
         where TEnum : struct where TEntity : class, IDictionaryWithTag, new()
        {
            return context.AddOrUpdateDictionary<TEnum, TEntity>(c =>
                              new TEntity
                              {
                                  Id = c.CastObject<int>(),
                                  Name = c.GetDescription(),
                                  Tag = c.ToString()
                              });
        }

        public static Task AddOrUpdateDictionary<TEnum, TEntity>(this IEntityFrameworkCoreDbContext context) where TEnum : struct where TEntity : class, IDictionary, new()
        {
            return context.AddOrUpdateDictionary<TEnum, TEntity>(c =>
                            new TEntity
                            {
                                Id = c.CastObject<int>(),
                                Name = c.ToString(),
                                Description = c.GetDescription()
                            });
        }

        public static async Task AddFromCsv<TEntity, TMap>(this IEntityFrameworkCoreDbContext context, string path) where TEntity : class where TMap : ClassMap<TEntity>, new()
        {
            if (!await context.Set<TEntity>().AnyAsync())
            {
                var data = CSVUtils.ParseCSV<TEntity, TMap>(path);

                context.Set<TEntity>().AddRange(data);
            }
        }
    }
}
