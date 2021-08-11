using PDCoreNew.Handlers.UserValidation;
using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Exceptions;

namespace PDCoreNew.Processors
{
    public class UserProcessor
    {
        public bool Register(User user)
        {
            try
            {
                var handler = new SocialSecurityNumberValidatorHandler();

                handler.SetNext(new AgeValidationHandler())
                    .SetNext(new NameValidationHandler())
                    .SetNext(new CitizenshipRegionValidationHandler());

                handler.Handle(user);
            }
            catch (UserValidationException)
            {
                return false;
            }

            return true;
        }
    }
}
