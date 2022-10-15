using System;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts
{
    public interface IImportProcessor<TModel, TSheetModel>
    {
        ValueTask Validate(TModel model, Func<string, Task<string[]>> getAllowedValuesFunc,
            Action<string, string> addError);
    }
}
