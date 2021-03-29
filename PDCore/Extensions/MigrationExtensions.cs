using Microsoft.EntityFrameworkCore.Migrations;
using PDCore.Utils;
using System;

namespace PDCore.Extensions
{
    public static class MigrationExtensions
    {
        public static void SqlResource(this MigrationBuilder migrationBuilder, Type migrationType, string sqlFileName)
        {
            var assembly = migrationType.Assembly;

            string content = MigrationUtils.ReadSql(assembly, assembly.GetName().Name, sqlFileName);

            migrationBuilder.Sql(content);
        }
    }
}
