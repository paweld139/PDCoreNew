using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Migrations.Internal;
using PDCoreNew.Extensions;

namespace PDCoreNew.Repositories.Repo
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    public class CustomNpsqlHistoryRepository : NpgsqlHistoryRepository
    {
        public CustomNpsqlHistoryRepository(HistoryRepositoryDependencies dependencies)
            : base(dependencies)
        {
        }

        protected override string TableName => base.TableName.ToSnakeCase();

        protected override void ConfigureTable(EntityTypeBuilder<HistoryRow> history)
        {
            base.ConfigureTable(history);

            history.HasKey(h => h.MigrationId).HasName($"pk_{TableName}");
            history.Property(h => h.MigrationId).HasColumnName(nameof(HistoryRow.MigrationId).ToSnakeCase());
            history.Property(h => h.ProductVersion).HasColumnName(nameof(HistoryRow.ProductVersion).ToSnakeCase());
        }
    }
}
