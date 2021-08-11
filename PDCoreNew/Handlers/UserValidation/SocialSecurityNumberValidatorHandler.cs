using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Exceptions;
using PDCoreNew.Validators;

namespace PDCoreNew.Handlers.UserValidation
{
    public class SocialSecurityNumberValidatorHandler : Handler<User>
    {
        private readonly SocialSecurityNumberValidator socialSecurityNumberValidator
            = new();

        public override void Handle(User request)
        {
            if (!socialSecurityNumberValidator.Validate(request.SocialSecurityNumber, request.CitizenshipRegion))
            {
                throw new UserValidationException("Social security number could not be valid");
            }

            base.Handle(request);
        }
    }
}
