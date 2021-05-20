using System.Collections.Generic;
using System.Data;

namespace PDCore.Repositories.IRepo
{
    public interface ISqlRepository<T> : IRepository<T>, ISqlRepository
    {
        List<T> GetByQuery(string query);

        List<T> GetByWhere(string where);


        string GetTableName();

        string GetQuery();

        string GetQuery(string where);

        int GetCountByWhere(string where);

        DataTable GetDataTableByWhere(string where);         
    }

    public interface ISqlRepository
    {
        bool IsLoggingEnabled { get; }

        void SetLogging(bool res);


        DataTable GetDataTableByQuery(string query);

        T GetValue<T>(string query);
    }
}
