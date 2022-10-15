using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts
{
    public interface IImportParser<TData, TModel, TSheetModel>
    {
        IList<TData> Parse(TSheetModel importSheetModel);
        Task<IList<TData>> ParseAsyncTask(TSheetModel importSheetModel);
    }
}
