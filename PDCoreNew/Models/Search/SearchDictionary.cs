using System.Collections.Generic;
using System.Linq;

namespace PDCoreNew.Models.Search
{
    public class SearchDictionary
    {
        public SearchDictionary()
        {
        }

        public SearchDictionary(IEnumerable<string> name, bool orderByKey = false, bool orderByValue = false)
        {
            Name = name;
            OrderByKey = orderByKey;
            OrderByValue = orderByValue;
        }

        public SearchDictionary(bool orderByKey = false, bool orderByValue = false, params string[] name)
            : this(name.AsEnumerable(), orderByKey, orderByValue)
        {
        }

        public IEnumerable<string> Name { get; set; }

        public bool OrderByKey { get; set; }

        public bool OrderByValue { get; set; }
    }
}
