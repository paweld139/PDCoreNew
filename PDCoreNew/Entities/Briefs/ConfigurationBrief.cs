using Newtonsoft.Json;
using PDCoreNew.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace PDCoreNew.Entities.Briefs
{
    public class ConfigurationBrief<TEnum> : IHasRowVersion where TEnum : struct
    {
        public ConfigurationBrief()
        {
        }

        public ConfigurationBrief(TEnum key)
        {
            Key = key.ToString();
        }

        public object Object
        {
            set
            {
                Value = JsonConvert.SerializeObject(value);
            }
        }

        public static ConfigurationBrief<TEnum> Create<T>(TEnum key, T model)
        {
            ConfigurationBrief<TEnum> configurationBrief = new(key);

            configurationBrief.Object = model;

            return configurationBrief;
        }

        public virtual string Key { get; set; }

        public virtual string Value { get; set; }

        [IgnoreDataMember]
        [NotMapped]
        public byte[] RowVersion { get; set; }
    }
}
