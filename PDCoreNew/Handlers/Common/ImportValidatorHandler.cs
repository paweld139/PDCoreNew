using PDCoreNew.Contracts;

namespace PDCoreNew.Handlers.Common
{
    public class ImportValidatorHandler<TSheetModel> : HandlerAsync<TSheetModel>
        where TSheetModel : class
    {
        public ImportValidatorHandler(params IReceiverAsync<TSheetModel>[] receiversAsync) : base(receiversAsync)
        {
        }
    }
}
