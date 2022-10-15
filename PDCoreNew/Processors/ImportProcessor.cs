using PDCoreNew.Contracts;
using PDCoreNew.Handlers.Common;
using System;
using System.Threading.Tasks;

namespace PDCoreNew.Processors
{
    public class ImportProcessor<TModel, TSheetModel> : IImportProcessor<TModel, TSheetModel>
           where TSheetModel : class
           where TModel : IHasFile
    {
        private readonly IReceiversFactory<TSheetModel> receiversFactory;
        private readonly IImportSheetModelFactory<TModel, TSheetModel> importSheetModelFactory;

        public ImportProcessor(IReceiversFactory<TSheetModel> receiversFactory, IImportSheetModelFactory<TModel, TSheetModel> importSheetModelFactory)
        {
            this.receiversFactory = receiversFactory;
            this.importSheetModelFactory = importSheetModelFactory;
        }

        public ValueTask Validate(TModel model, Func<string, Task<string[]>> getAllowedValuesFunc,
            Action<string, string> addError)
        {
            var receivers = receiversFactory.Create(addError);

            var handler = new ImportValidatorHandler<TSheetModel>(receivers);

            var request = importSheetModelFactory.Create(model, getAllowedValuesFunc);

            return handler.HandleAsync(request);
        }
    }
}
