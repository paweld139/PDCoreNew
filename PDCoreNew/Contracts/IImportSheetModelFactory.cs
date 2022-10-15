using System;
using System.Threading.Tasks;

namespace PDCoreNew.Contracts
{
    public interface IImportSheetModelFactory<TModel, TResult> : IFactory<TResult>
        where TResult : class
        where TModel : IHasFile
    {
        TResult Create(TModel model, Func<string, Task<string[]>> getAllowedValuesFunc);
    }
}
