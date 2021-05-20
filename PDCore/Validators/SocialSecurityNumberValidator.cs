using System.Globalization;

namespace PDCore.Validators
{
    public class SocialSecurityNumberValidator
    {
        public bool Validate(string socialSecurityNumber, RegionInfo citizenshipRegion)
        {
            _ = socialSecurityNumber;

            _ = citizenshipRegion;

            return true;
        }
    }
}
