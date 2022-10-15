using PDCoreNew.Contracts;
using PDWebCoreNew.Extensions;
using System;
using System.Threading.Tasks;

namespace PDCoreNew.Factories.Fac
{
    public class ImportSheetModelFactory<TModel, TResult> : Factory<TResult>, IImportSheetModelFactory<TModel, TResult>
        where TResult : class
        where TModel : IHasFile
    {
        private const int HeaderRowInt = 4;

        public override TResult Create(params object[] parameters)
        {
            TResult result = null;

            if (parameters[0] is TModel model &&
                parameters[1] is Func<string, Task<string[]>> getAllowedValuesFunc)
            {
                result = Create(model, getAllowedValuesFunc);
            }

            return result;
        }

        public TResult Create(TModel model, Func<string, Task<string[]>> getAllowedValuesFunc)
        {
            var file = model.File;

            var sheet = file.GetSheet();

            return (TResult)Activator.CreateInstance(typeof(TResult), model, sheet, HeaderRowInt, getAllowedValuesFunc);
        }
    }
}
