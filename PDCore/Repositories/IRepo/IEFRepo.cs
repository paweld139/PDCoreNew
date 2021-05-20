﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PDCore.Repositories.IRepo
{
    public interface IEFRepo<T> : PDCore.Repositories.IRepo.IRepo<T> where T : class
    {
        Task<List<T>> GetAllAsync(bool asNoTracking = true);

        IQueryable<T> Get(bool asNoTracking = true);

        Task<int> SaveChangesAsync();

        void SaveChanges();

        Task<T> LoadAsync(int id);

        void Attach(T obj);
    }
}
