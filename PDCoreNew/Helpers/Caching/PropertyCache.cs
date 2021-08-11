using System;
using System.Collections.Generic;
using System.Reflection;

namespace PDCoreNew.Helpers
{
    public static class PropertyCache<T>
    {
        private static readonly Lazy<IEnumerable<PropertyInfo>> publicPropertiesLazy = new(
            () => Array.AsReadOnly(typeof(T).GetProperties()));

        public static IEnumerable<PropertyInfo> PublicProperties => publicPropertiesLazy.Value;
    }
}
