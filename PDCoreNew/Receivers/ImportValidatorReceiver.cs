using Microsoft.Extensions.Localization;
using PDCoreNew.Contracts;
using PDCoreNew.Contracts.Validators;
using PDCoreNew.Extensions;
using System;
using System.Threading.Tasks;

namespace PDCoreNew.Receivers
{
    public class ImportValidatorReceiver<TSheetModel> : IReceiverAsync<TSheetModel>
           where TSheetModel : class
    {
        private readonly IValidator<TSheetModel> validator;
        private readonly IStringLocalizer stringLocalizer;
        private readonly Action<string, string> addError;

        public ImportValidatorReceiver(IValidator<TSheetModel> validator, IStringLocalizer stringLocalizer,
            Action<string, string> addError)
        {
            this.validator = validator;
            this.stringLocalizer = stringLocalizer;
            this.addError = addError;
        }

        public void Handle(TSheetModel request)
        {
            HandleAsync(request).AsTask().Wait();
        }

        public async ValueTask HandleAsync(TSheetModel request)
        {
            bool isValid = await validator.Validate(request);

            isValid.AddError(addError, () => validator.GetError(stringLocalizer));
        }
    }
}
