using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Exceptions;

namespace PDCoreNew.Handlers.UserValidation
{
    public class CitizenshipRegionValidationHandler : Handler<User>
    {
        public override void Handle(User user)
        {
            if (user.CitizenshipRegion.TwoLetterISORegionName == "NO")
            {
                throw new UserValidationException("We currently not support Norwegians");
            }

            base.Handle(user);
        }
    }
}
