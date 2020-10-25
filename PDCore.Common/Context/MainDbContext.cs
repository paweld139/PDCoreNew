using PDCore.Common.Context.IContext;
using PDCore.Common.Extensions;
using PDCore.Interfaces;
using PDCore.Models;
using System.Data.Entity;

namespace PDCore.Common.Context
{
    public abstract class MainDbContext : DbContext, IMainDbContext
    {
        protected MainDbContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<LogModel> ErrorLog { get; set; }

        public DbSet<FileModel> File { get; set; }

        public bool IsLoggingEnabled => this.IsLoggingEnabled();

        public void SetLogging(bool input, ILogger logger)
        {
            this.SetLogging(input, logger, IsLoggingEnabled);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.ConfigureForModificationHistory();

            base.OnModelCreating(modelBuilder);
        }
    }
}
