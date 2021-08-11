using Microsoft.EntityFrameworkCore;
using PDCoreNew.Context.IContext;
using PDCoreNew.Extensions;
using PDCoreNew.Interfaces;
using System;

namespace PDCoreNew.Context
{
    public abstract class MainDbContext : DbContext, IEntityFrameworkCoreDbContext
    {
        protected MainDbContext(DbContextOptions options) : base(options)
        {
        }

        public bool IsLoggingEnabled => throw new NotImplementedException();

        public void SetLogging(bool input, ILogger logger)
        {
            throw new NotImplementedException();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureForModificationHistory();

            base.OnModelCreating(modelBuilder);
        }
    }
}
