using PDCoreNew.Utils;
using System;
using System.Globalization;

namespace PDCoreNew.Models.Shop
{
    public class User
    {
        public User(string name, string socialSecurityNumber, RegionInfo citizenshipRegion, DateTimeOffset birthDate)
        {
            Name = name;
            SocialSecurityNumber = socialSecurityNumber;
            CitizenshipRegion = citizenshipRegion;
            BirthDate = birthDate;
            Age = DateTimeUtils.CalculateAge(BirthDate.Date);
        }

        public string Name { get; set; }
        public string SocialSecurityNumber { get; set; }
        public RegionInfo CitizenshipRegion { get; set; }
        public int Age { get; set; }
        public DateTimeOffset BirthDate { get; set; }
    }
}
