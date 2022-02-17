using PDCoreNew.Entities.Briefs;
using PDCoreNew.Interfaces;
using System;
using System.Runtime.Serialization;

namespace PDCoreNew.Entities
{
    public class Configuration<TEnum> : ConfigurationBrief<TEnum>, IModificationHistory where TEnum : struct
    {
        public Configuration()
        {
        }

        public Configuration(TEnum key, string value) : base(key)
        {
            Value = value;
        }


        #region ModificationHistory

        [IgnoreDataMember]
        public DateTime DateModified { get; set; }

        [IgnoreDataMember]
        public DateTime DateCreated { get; set; }

        [IgnoreDataMember]
        public bool IsDirty { get; set; }

        #endregion
    }
}
