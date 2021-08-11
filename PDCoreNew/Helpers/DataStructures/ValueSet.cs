using PDCoreNew.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PDCoreNew.Helpers.DataStructures
{
    public abstract class ValueSet<TEnum> where TEnum : struct
    {
        private Dictionary<TEnum, ValueSetItem> Dictionary
        {
            get
            {
                return GetDictionary();
            }
        }

        protected abstract Dictionary<TEnum, ValueSetItem> GetDictionary();


        public ValueSetItem this[TEnum enumItem]
        {
            get
            {
                return Dictionary[enumItem];
            }
        }

        public List<KeyValuePair<TEnum, string>> GetKeyValuePairs()
        {
            return Dictionary.GetKVP(x => x.Key, x => (x.Value.DisplayName ?? x.Value.Code)).OrderBy(x => x.Value).ToList();
        }

        public List<KeyValuePair<TEnum?, string>> GetKeyValuePairsWithNullableKey()
        {
            var result = Dictionary.GetKVP(x => (TEnum?)x.Key, x => (x.Value.DisplayName ?? x.Value.Code)).OrderBy(x => x.Value).ToList();

            result.Insert(0, new KeyValuePair<TEnum?, string>(null, Resources.Common.All));


            return result;
        }

        public List<KeyValuePair<int, string>> GetKeyValuePairsInt()
        {
            return GetKeyValuePairs().GetKVPList(e => Convert.ToInt32(e.Key), e => e.Value);
        }
    }

    public class ValueSetItem
    {
        public string Code { get; private set; }

        public string DisplayName { get; private set; }

        public override string ToString() => DisplayName ?? Code;

        public ValueSetItem(string displayName, string code = null)
        {
            DisplayName = displayName;

            Code = code;
        }
    }
}
