using AutoMapper;
using PDCore.Common.Context.IContext;
using PDCore.Extensions;
using PDCore.Interfaces;
using PDCore.Repositories.IRepo;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Threading.Tasks;

namespace PDCore.Common.Repositories.Repo
{
    public class SqlRepositoryEntityFrameworkConnected<T> :
        SqlRepositoryEntityFrameworkAsync<T>, ISqlRepositoryEntityFrameworkConnected<T> where T : class, IModificationHistory, new()
    {
        public SqlRepositoryEntityFrameworkConnected(IEntityFrameworkDbContext ctx, ILogger logger, IMapper mapper) : base(ctx, logger, mapper)
        {
        }

        //Funkcjonalność ConnectedRepository. Nie ma potrzeby pobierania danych wiele razy. Repository i kontekst żyją w danym oknie.
        public virtual ObservableCollection<T> GetAllFromMemory()
        {
            if (set.Local.IsEmpty())
            {
                GetAll();
            }

            return set.Local;
        }

        //Funkcjonalność ConnectedRepository. Nie ma potrzeby pobierania danych wiele razy. Repository i kontekst żyją w danym oknie.
        public virtual async Task<ObservableCollection<T>> GetAllFromMemoryAsync()
        {
            if (set.Local.IsEmpty())
            {
                await GetAllAsync();
            }

            return set.Local;
        }


        //Funkcjonalność ConnectedRepository, np. do Bindingu obiektu w WPF.
        public virtual T Add()
        {
            var entry = new T();

            Add(entry);

            return entry;
        }


        //Funkcjonalność ConnectedRepository. Pozbycie się z pamięci przed zapisem obiektów utworzonych, ale niezedytowanych.
        private void RemoveEmptyEntries()
        {
            T entry;

            //you can't remove from or add to a collection in a foreach loop
            for (int i = set.Local.Count; i > 0; i--)
            {
                entry = set.Local[i - 1];

                if (ctx.Entry(entry).State == EntityState.Added && !entry.IsDirty)
                {
                    Delete(entry);
                }
            }
        }

        public override int Commit()
        {
            RemoveEmptyEntries();

            return base.Commit();
        }

        public override Task<int> CommitAsync()
        {
            RemoveEmptyEntries();

            return base.CommitAsync();
        }

        public T AddAndReturn(T entity)
        {
            Add(entity);

            return entity;
        }
    }
}
