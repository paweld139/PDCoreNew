using System;
using System.Data.Entity.Validation;
using System.Linq;

namespace PDCore.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetErrors(this DbEntityValidationException e)
        {
            return string.Join(Environment.NewLine, e.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => x.ErrorMessage));
        }
    }
}
