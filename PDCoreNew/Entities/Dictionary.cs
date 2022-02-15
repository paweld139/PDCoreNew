using PDCoreNew.Entities.Basic;
using PDCoreNew.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PDCoreNew.Entities
{
    public class Dictionary : DictionaryBasic, IModificationHistory
    {
        public int Id { get; set; }

        public string Description { get; set; }


        #region ModificationHistory

        public DateTime DateModified { get; set; }

        public DateTime DateCreated { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [IgnoreDataMember]
        public bool IsDirty { get; set; }

        #endregion
    }
}
