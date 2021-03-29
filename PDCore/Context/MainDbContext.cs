using Microsoft.EntityFrameworkCore;
using PDCore.Context.IContext;
using PDCore.Extensions;
using PDCore.Interfaces;
using System;

namespace PDCore.Context
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
