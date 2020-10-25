using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using PDCore.Extensions;

namespace PDCore.Common.Utils
{
    public static class SqlUtils
    {
        public static DbProviderFactory GetDbProviderFactory(string provider)
        {
            DbProviderFactory dbProviderFactory = null;

            if (!string.IsNullOrEmpty(provider))
            {
                dbProviderFactory = DbProviderFactories.GetFactory(provider);
            }

            return dbProviderFactory;
        }

        public static DbConnection GetDbConnection(string nameOrConnectionString, bool open, string provider = null)
        {
            DbProviderFactory dbProviderFactory = GetDbProviderFactory(provider);

            return PDCore.Utils.SqlUtils.GetDbConnection(nameOrConnectionString, open, dbProviderFactory);
        }

        public static bool TestConnectionString(string text, string provider = null)
        {
            DbProviderFactory dbProviderFactory = GetDbProviderFactory(provider);

            return PDCore.Utils.SqlUtils.TestConnectionString(text, dbProviderFactory);
        }

        public static IEnumerable<string> GetTables(string nameOrConnectionString, string provider = null)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true, provider))
            {
                return PDCore.Utils.SqlUtils.GetTables(dbConnection);
            }
        }

        public static DataSet GetDataSet(string query, string nameOrConnectionString, string provider = null)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true, provider))
            {
                return PDCore.Utils.SqlUtils.GetDataSet(query, dbConnection);
            }
        }

        public static DataTable GetDataTable(string query, string nameOrConnectionString, string provider = null)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true, provider))
            {
                return PDCore.Utils.SqlUtils.GetDataTable(query, dbConnection);
            }
        }
    }
}
