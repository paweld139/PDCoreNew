using CommonServiceLocator;
using PDCore.Common.Context.IContext;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using System;
using System.Threading.Tasks;

namespace PDCore.Common.UnitOfWork
{
    public abstract class UnitOfWork2 : IUnitOfWork
    {
        private readonly IEntityFrameworkDbContext context;

        protected UnitOfWork2(IEntityFrameworkDbContext context)
        {
            this.context = context;

            PrepareDbContext();
        }

        // Repositories

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            context.SaveChanges();
        }

        public Task CommitAsync()
        {
            //System.Diagnostics.Debug.WriteLine("Committed");
            return context.SaveChangesAsync();
        }

        private void PrepareDbContext()
        {
            // Do NOT enable proxied entities, else serialization fails
            context.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            //dbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            //dbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        protected ISqlRepositoryEntityFramework<T> GetStandardRepo<T>() where T : class, IModificationHistory
        {
            return GetRepo<ISqlRepositoryEntityFramework<T>>();
        }

        protected ISqlRepositoryEntityFrameworkDisconnected<T> GetStandardRepoDisconnected<T>() where T : class, IModificationHistory
        {
            return GetRepo<ISqlRepositoryEntityFrameworkDisconnected<T>>();
        }

        protected ISqlRepositoryEntityFrameworkConnected<T> GetStandardRepoConnected<T>() where T : class, IModificationHistory, new()
        {
            return GetRepo<ISqlRepositoryEntityFrameworkConnected<T>>();
        }

        protected T GetRepo<T>() where T : class
        {
            return ServiceLocator.Current.GetInstance<T>();
        }


        public void CleanUp()
        {
            throw new NotImplementedException();
        }

        public void CleanUp<TEntity>() where TEntity : class
        {
            throw new NotImplementedException();
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
                if (context != null)
                {
                    context.Dispose();
                }
            }
        }

        #endregion
    }
}
