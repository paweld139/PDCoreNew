using System.Globalization;

namespace PDCoreNew.Validators
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
