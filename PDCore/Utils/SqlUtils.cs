﻿using PDCore.Extensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace PDCore.Utils
{
    public static class SqlUtils
    {
        public const string CountSelection = "count(*)";


        public static string SQLQuery(string objectName = null, string projection = "*", string selection = null, string joins = "", string order = null,
            string orderType = "", bool isUpdate = false, bool isInsert = false, bool isDelete = false, bool isStoredProcedure = false, bool isFunction = false, string parameters = "")
        {
            //StringBuilder q = new StringBuilder("set dateformat ymd; ");
            StringBuilder q = new StringBuilder();

            if (isStoredProcedure)
            {
                q.AppendFormat("exec {0} {1}", objectName, parameters);
            }
            else if (isFunction)
            {
                q.AppendFormat("select {0} from {1}({2})", projection, objectName, parameters);
            }
            else
            {
                if (isUpdate)
                {
                    q.AppendFormat("update {0} set {1}", objectName, projection);
                }
                else if (isInsert)
                {
                    q.AppendFormat("insert into {0} ({1}) values ({2})", objectName, projection, selection);
                }
                else if (isDelete)
                {
                    q.AppendFormat("delete from {0}", objectName);
                }
                else
                {
                    q.AppendFormat("select {0} from {1} {2}", projection, objectName, joins);
                }

                if (!isInsert && !string.IsNullOrWhiteSpace(selection))
                {
                    q.AppendFormat(" where {0}", selection);
                }
            }

            if (!string.IsNullOrWhiteSpace(order))
            {
                q.AppendFormat(" order by {0} {1}", order, orderType);
            }

            q.Append(";");

            string query = q.ToString();

            return query;
        }

        public static Expression<Func<T, bool>> GetFindByDateExpression<T>(string dateF, string dateT, Expression<Func<T, DateTime>> dateSelector)
        {
            Expression<Func<T, bool>> expression = null;

            if (!StringUtils.AreNullOrWhiteSpace(dateF, dateT))
            {
                DateTime.TryParse(dateF, out DateTime dateFrom);

                DateTime.TryParse(dateT, out DateTime dateTo);

                if (dateTo != DateTime.MinValue)
                {
                    dateTo = dateTo.AddDays(1).AddTicks(-1);
                }

                string datePropertyName = ReflectionUtils.GetNameOf(dateSelector);

                var itemParam = Expression.Parameter(typeof(T), "item");
                var itemPropertyExpr = Expression.Property(itemParam, datePropertyName);
                var dateFromConstant = Expression.Constant(dateFrom);
                var dateToConstant = Expression.Constant(dateTo);

                BinaryExpression binaryExpression = null;

                if (StringUtils.AreNotNullOrWhiteSpace(dateF, dateT))
                {
                    if (dateTo > dateFrom)
                    {
                        binaryExpression = Expression.MakeBinary(ExpressionType.And,
                                           Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, itemPropertyExpr, dateFromConstant),
                                           Expression.MakeBinary(ExpressionType.LessThanOrEqual, itemPropertyExpr, dateToConstant));
                    }
                }
                else if (string.IsNullOrWhiteSpace(dateF))
                {
                    binaryExpression = Expression.MakeBinary(ExpressionType.LessThanOrEqual, itemPropertyExpr, dateToConstant);
                }
                else
                {
                    binaryExpression = Expression.MakeBinary(ExpressionType.GreaterThanOrEqual, itemPropertyExpr, dateFromConstant);
                }

                if (binaryExpression != null)
                {
                    expression = Expression.Lambda(binaryExpression, itemParam) as Expression<Func<T, bool>>;
                }
            }

            return expression;
        }

        public static Expression<Func<T, bool>> GetFindByDateExpression<T>(DateTime? dateF, DateTime? dateT, Expression<Func<T, DateTime>> dateSelector)
        {
            return GetFindByDateExpression(dateF?.ToString(), dateT?.ToString(), dateSelector);
        }

        public static void FindByDate<T>(string dateF, string dateT, Expression<Func<T, DateTime>> dateSelector, ref IQueryable<T> result) where T : class
        {
            if (result == null)
            {
                return;
            }

            var expression = GetFindByDateExpression(dateF, dateT, dateSelector);

            if (expression != null)
            {
                result = result.Where(expression);
            }
        }

        public static void FindByDate<T>(DateTime? dateF, DateTime? dateT, Expression<Func<T, DateTime>> dateSelector, ref IQueryable<T> result) where T : class
        {
            FindByDate(dateF?.ToString(), dateT?.ToString(), dateSelector, ref result);
        }

        //public static LocalSqlHelper EnableSQLConnection()
        //{
        //    LocalSqlHelper helper = new LocalSqlHelper(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);

        //    return helper;
        //}

        public static string GetSaveChangesResults(params string[] results)
        {
            string result = WebUtils.ResultOkIndicator;

            foreach (string item in results)
            {
                if (item != result)
                {
                    result = item;

                    break;
                }
            }

            return result;
        }

        public static string GetConnectionString(string nameOrConnectionString)
        {
            string result = !nameOrConnectionString.IsConnectionString() ? ConfigurationManager.ConnectionStrings[nameOrConnectionString]?.ConnectionString
                            : nameOrConnectionString;

            return result;
        }

        /// <summary>
        /// Construct a DataAdapater based on the type of DbConnection passed.
        /// You can call connection.CreateCommand() to create a DbCommand object,
        /// but there's no corresponding connection.CreateDataAdapter() method.
        /// </summary>
        /// <param name="connection"></param>
        /// <exception>Throws Exception if the connection is not of a known type.</exception>
        /// <returns></returns>
        public static DbDataAdapter CreateDataAdapter(DbConnection connection)
        {
            //Note: Any code is released into the public domain. No attribution required.

            DbDataAdapter adapter; //we can't construct an adapter directly
                                   //So let's run around the block 3 times, before potentially crashing

            if (connection is SqlConnection)
                adapter = new SqlDataAdapter();
            else if (connection is OleDbConnection)
                adapter = new OleDbDataAdapter();
            else if (connection is OdbcConnection)
                adapter = new OdbcDataAdapter();
            //else if (connection is System.Data.SqlServerCe.SqlCeConnection)
            //    adapter = new System.Data.SqlServerCe.SqlCeDataAdapter();
            //else if (connection is Oracle.ManagedDataAccess.Client.OracleConnection)
            //    adapter = new Oracle.ManagedDataAccess.Client.OracleDataAdapter();
            //else if (connection is Oracle.DataAccess.Client.OracleConnection)
            //    adapter = new Oracle.DataAccess.Client.OracleDataAdapter();
            //else if (connection is IBM.Data.DB2.DB2Connection)
            //    adapter = new IBM.Data.DB2.DB2DataAdapter();
            ////TODO: Add more DbConnection kinds as they become invented
            else
            {
                throw new Exception("[CreateDataAdapter] Unknown DbConnection type: " + connection.GetType().FullName);
            }

            return adapter;
        }

        public static string GetTableName(string query)
        {
            /*
             * (?i) - włączenie case insensitive dla FROM
             * (?-i) - wyłączenie case insensitive
             * ?<table> - utworzenie grupy nazwanej "table"
             * .* - 0 lub więcej każdego znaku oprócz znaku nowej linii
             * ( AS|where) - biały znak, później "AS" lub "where" 0 razy lub raz
             */
            Regex regex = new Regex(@"(?i)from\s+(?<table>((\[.+\](?=\s+(as|where|inner|left|right|join|union)))|\[.+\]\s*$|\S*))");
            Match match = regex.Match(query);

            string table = match.Groups["table"].Value;

            return table;
        }

        public static string GetCountQuery(string tableName, string where = null)
        {
            string query = SQLQuery(tableName, CountSelection, where);

            return query;
        }

        public static IEnumerable<string> GetTables(DbConnection dbConnection)
        {
            DataTable dt = dbConnection.GetSchema("Tables");

            var tableNames = dt.GetColumnStringValues(2);

            return tableNames;
        }

        public static DbDataAdapter CreateDataAdapter(DbCommand cmd)
        {
            /*
             * DbProviderFactories.GetFactory(DbConnection connection) seams buggy 
             * (.NET Framework too old?)
             * this is a workaround
             */

            DbProviderFactory factory = GetDbProviderFactory(cmd.Connection);

            DbDataAdapter adapter = factory.CreateDataAdapter();

            adapter.SelectCommand = cmd;

            return adapter;
        }

        public static DbProviderFactory GetDbProviderFactory(DbConnection connection)
        {
            DbProviderFactory providerFactory = null;
            Type connectionType = connection.GetType();
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            // Check for DbConnection.DbProviderFactory
            PropertyInfo providerFactoryInfo = connectionType.GetProperty("DbProviderFactory", flags);
            if (providerFactoryInfo != null)
                providerFactory = (DbProviderFactory)providerFactoryInfo.GetValue(connection, null);

            // Check for DbConnection.ConnectionFactory.ProviderFactory
            if (providerFactory == null)
            {
                PropertyInfo connectionFactoryInfo = connectionType.GetProperty("ConnectionFactory", flags);
                object connectionFactory = connectionFactoryInfo.GetValue(connection, null);
                providerFactoryInfo = connectionFactory.GetType().GetProperty("ProviderFactory", flags);
                providerFactory = (DbProviderFactory)providerFactoryInfo.GetValue(connectionFactory, null);
            }

            // No match
            if (providerFactory == null)
                throw new InvalidOperationException("ProviderFactory not found for connection");

            return providerFactory;
        }

        public static DataSet GetDataSet(string query, DbConnection dbConnection)
        {
            DataSet dataSet = new DataSet();

            DbProviderFactory dbProviderFactory = GetDbProviderFactory(dbConnection);

            using (DbCommand dbCommand = dbProviderFactory.CreateCommand())
            {
                dbCommand.Connection = dbConnection;
                dbCommand.CommandType = CommandType.Text;
                dbCommand.CommandText = query;

                using (DbDataAdapter dbDataAdapter = dbProviderFactory.CreateDataAdapter())
                {
                    dbDataAdapter.SelectCommand = dbCommand;

                    dbDataAdapter.Fill(dataSet);
                }
            }

            return dataSet;
        }

        public static DataTable GetDataTable(string query, DbConnection dbConnection)
        {
            DataSet dataSet = GetDataSet(query, dbConnection);

            if (dataSet.Tables.Count > 0)
                return dataSet.Tables[0];

            return new DataTable();
        }

        public static string GetDefaultSchema(SqlConnection sqlConnection)
        {
            const string defaultSchemaQuery = "select SCHEMA_NAME() as DefaultSchema";

            DataTable result = GetDataTable(defaultSchemaQuery, sqlConnection);

            return result.GetValue();
        }

        public static DbConnection GetDbConnection(string nameOrConnectionString, bool open, DbProviderFactory factory = null)
        {
            string connectionString = GetConnectionString(nameOrConnectionString);

            DbConnection dbConnection;

            if (factory != null) //"System.Data.SqlClient"; // for example
            {
                dbConnection = factory.CreateConnection();

                dbConnection.ConnectionString = connectionString;
            }
            else
            {
                dbConnection = new SqlConnection(connectionString);
            }

            if (open)
                dbConnection.OpenConnectionIfClosed();


            return dbConnection;
        }

        public static bool TestConnectionString(string text, DbProviderFactory factory = null)
        {
            bool isConnectionString = text.IsConnectionString();

            if (isConnectionString)
            {
                var dbConnection = GetDbConnection(text, false, factory);

                try
                {
                    dbConnection.Open(); // throws if invalid
                }
                catch
                {
                    return false;
                }
                finally
                {
                    dbConnection.Dispose();
                }
            }

            return isConnectionString;
        }

        public static IEnumerable<string> GetTables(string nameOrConnectionString)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true))
            {
                return GetTables(dbConnection);
            }
        }

        public static DataSet GetDataSet(string query, string nameOrConnectionString)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true))
            {
                return GetDataSet(query, dbConnection);
            }
        }

        public static DataTable GetDataTable(string query, string nameOrConnectionString)
        {
            using (DbConnection dbConnection = GetDbConnection(nameOrConnectionString, true))
            {
                return GetDataTable(query, dbConnection);
            }
        }

        public static DataTable GetDataTable(string query)
        {
            string connectionString = GetConnectionString("DefaultConnection");

            return GetDataTable(query, connectionString);
        }

        public static DataSet GetDataSet(string query)
        {
            string connectionString = GetConnectionString("DefaultConnection");

            return GetDataSet(query, connectionString);
        }
    }
}
