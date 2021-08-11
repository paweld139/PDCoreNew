using System;
using System.Collections.Generic;
using System.Linq;

namespace PDCoreNew.Attributes
{
    public class EnsureUpdatedAttribute : Attribute
    {
        public IEnumerable<string> Properties { get; private set; }

        public EnsureUpdatedAttribute(params string[] properties)
        {
            Properties = properties.AsEnumerable();
        }
    }
}
