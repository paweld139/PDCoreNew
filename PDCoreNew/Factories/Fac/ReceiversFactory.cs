using Microsoft.Extensions.Localization;
using PDCoreNew.Contracts;
using PDCoreNew.Extensions;
using PDCoreNew.Receivers;
using System;

namespace PDCoreNew.Factories.Fac
{
    public class ReceiversFactory<TSheetModel> : Factory<IReceiverAsync<TSheetModel>[]>, IReceiversFactory<TSheetModel>
           where TSheetModel : class
    {
        private readonly IValidatorFactory<TSheetModel> validatorFactory;
        private readonly IStringLocalizer<Resource> stringLocalizer;

        public ReceiversFactory(IValidatorFactory<TSheetModel> validatorFactory, IStringLocalizer<Resource> stringLocalizer)
        {
            this.validatorFactory = validatorFactory;
            this.stringLocalizer = stringLocalizer;
        }

        public override IReceiverAsync<TSheetModel>[] Create(params object[] parameters)
        {
            IReceiverAsync<TSheetModel>[] result = null;

            if (parameters[0] is Action<string, string> addError)
            {
                result = Create(addError);
            }

            return result;
        }

        public IReceiverAsync<TSheetModel>[] Create(Action<string, string> addError)
        {
            var validators = validatorFactory.GetAll();

            return validators.ToArray(v => new ImportValidatorReceiver<TSheetModel>(v, stringLocalizer, addError));
        }
    }
}
