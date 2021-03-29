using PDCore.Common.Context.IContext;
using PDCore.Common.Extensions;
using PDCore.Common.Factories.Fac.Repository;
using PDCore.Interfaces;
using PDCore.Models;
using PDCore.Repositories.IRepo;
using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace PDCore.Common.UnitOfWork
{

    /// <summary>
    /// The "Unit of Work"
    ///     1) decouples the repos from the controllers
    ///     2) decouples the DbContext and EF from the controllers
    ///     3) manages the UoW
    /// </summary>
    /// <remarks>
    /// This class implements the "Unit of Work" pattern in which
    /// the "UoW" serves as a facade for querying and saving to the database.
    /// Querying is delegated to "repositories".
    /// Each repository serves as a container dedicated to a particular
    /// root entity type such as a <see cref="LogModel"/>.
    /// A repository typically exposes "Get" methods for querying and
    /// will offer add, update, and delete methods if those features are supported.
    /// The repositories rely on their parent UoW to provide the interface to the
    /// data layer (which is the EF DbContext).
    /// </remarks>
    public abstract class UnitOfWork : IUnitOfWork //<TContext> : IUnitOfWork where TContext : IEntityFrameworkDbContext
    {
        private IRepositoryProvider RepositoryProvider { get; set; }

        private readonly IEntityFrameworkDbContext dbContext;

        protected UnitOfWork(IRepositoryProvider repositoryProvider, IEntityFrameworkDbContext dbContext)
        {
            //CreateDbContext();

            RepositoryProvider = repositoryProvider;
            repositoryProvider.DbContext = dbContext;
            this.dbContext = dbContext;

            PrepareDbContext();
        }

        // Repositories

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            dbContext.SaveChangesWithModificationHistory();
        }

        public Task CommitAsync()
        {
            return dbContext.SaveChangesWithModificationHistoryAsync();
        }

        private void PrepareDbContext()
        {
            dbContext.Configuration.ProxyCreationEnabled = false;

            dbContext.Configuration.LazyLoadingEnabled = false;

            //dbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        //protected void CreateDbContext()
        //{
        //    DbContext = Activator.CreateInstance<TContext>();

        //    // Do NOT enable proxied entities, else serialization fails
        //    DbContext.Configuration.ProxyCreationEnabled = false;

        //    // Load navigation properties explicitly (avoid serialization trouble)
        //    DbContext.Configuration.LazyLoadingEnabled = false;

        //    // Because Web API will perform validation, we don't need/want EF to do so
        //    DbContext.Configuration.ValidateOnSaveEnabled = false;

        //    //DbContext.Configuration.AutoDetectChangesEnabled = false;
        //    // We won't use this performance tweak because we don't need 
        //    // the extra performance and, when autodetect is false,
        //    // we'd have to be careful. We're not being that careful.
        //}

        protected ISqlRepositoryEntityFramework<T> GetStandardRepo<T>() where T : class, IModificationHistory
        {
            return RepositoryProvider.GetRepositoryForEntityType<T>();
        }

        protected ISqlRepositoryEntityFrameworkConnected<T> GetStandardRepoConnected<T>() where T : class, IModificationHistory, new()
        {
            return RepositoryProvider.GetRepositoryForEntityTypeConnected<T>();
        }

        protected ISqlRepositoryEntityFrameworkDisconnected<T> GetStandardRepoDisconnected<T>() where T : class, IModificationHistory
        {
            return RepositoryProvider.GetRepositoryForEntityTypeDisconnected<T>();
        }

        protected T GetRepo<T>() where T : class
        {
            return RepositoryProvider.GetRepository<T>();
        }


        public void CleanUp()
        {
            foreach (DbEntityEntry dbEntityEntry in dbContext.ChangeTracker.Entries().ToArray())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }

        public void CleanUp<TEntity>() where TEntity : class
        {
            foreach (DbEntityEntry dbEntityEntry in dbContext.ChangeTracker.Entries<TEntity>())
            {
                if (dbEntityEntry.Entity != null)
                {
                    dbEntityEntry.State = EntityState.Detached;
                }
            }
        }


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
