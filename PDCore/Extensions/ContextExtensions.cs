using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using PDCore.Context.IContext;
using PDCore.Enums;
using PDCore.Interfaces;
using PDCore.Utils;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace PDCore.Extensions
{
    public static class ContextExtensions
    {
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();

        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");

        private static readonly FieldInfo QueryModelGeneratorField = QueryCompilerTypeInfo.DeclaredFields.First(x => x.Name == "_queryModelGenerator");

        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");

        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static string ToSql<TEntity>(this IQueryable<TEntity> query) where TEntity : class
        {
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(query.Provider);
            var modelGenerator = (QueryModelGenerator)QueryModelGeneratorField.GetValue(queryCompiler);
            var queryModel = modelGenerator.ParseQuery(query.Expression);
            var database = (IDatabase)DataBaseField.GetValue(queryCompiler);
            var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
            var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
            var modelVisitor = (RelationalQueryModelVisitor)queryCompilationContext.CreateQueryModelVisitor();
            modelVisitor.CreateQueryExecutor<TEntity>(queryModel);
            var sql = modelVisitor.Queries.First().ToString();

            return sql;
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
    }
}
