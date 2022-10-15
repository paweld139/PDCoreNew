using System;

namespace PDCoreNew.Extensions
{
    public static class BoolExtensions
    {
        public static void AddError(this bool isValid, Action<string, string> addError, Func<string> getError)
        {
            if (!isValid)
            {
                string error = getError();

                addError(string.Empty, error);
            }
        }
    }
}
