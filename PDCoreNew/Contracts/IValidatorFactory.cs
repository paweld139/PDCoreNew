using PDCoreNew.Contracts.Validators;
using System.Collections.Generic;

namespace PDCoreNew.Contracts
{
    public interface IValidatorFactory<T>
    {
        IEnumerable<IValidator<T>> GetAll();
    }
}
