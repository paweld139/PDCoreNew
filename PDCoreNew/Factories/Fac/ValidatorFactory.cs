using PDCoreNew.Contracts;
using PDCoreNew.Contracts.Validators;
using PDCoreNew.Validators.Common;
using System.Collections.Generic;
using System.Linq;

namespace PDCoreNew.Factories.Fac
{
    public class ValidatorFactory<TSheetModel> : FactoryProvider<Validator<object>, IValidator<TSheetModel>>, IValidatorFactory<TSheetModel>
    {
        public override IValidator<TSheetModel> CreateFactoryFor(params object[] parameters) => null;

        public IEnumerable<IValidator<TSheetModel>> GetAll()
        {
            var validators = GetAllFactories();

            return validators.OrderBy(v => v.Order);
        }
    }
}
