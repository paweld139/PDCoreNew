using System;

namespace PDCoreNew.Models.Shop.Exceptions
{
    public class UserValidationException : SystemException
    {
        public UserValidationException(string message) : base(message)
        {
        }
    }
}
