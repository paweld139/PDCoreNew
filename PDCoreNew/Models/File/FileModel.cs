using PDCoreNew.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace PDCoreNew.Models
{
    public class FileModel : FileBrief, IModificationHistory
    {
        [DataType(DataType.Text)]
        [LocalizedStringLengthMinMax(0, 100)]
        public string MimeType { get; set; }

        [LocalizedRequired]
        public int RefId { get; set; }

        public ObjType RefGid { get; set; }

        public string Description { get; set; }

        public int GroupId { get; set; }

        [NotMapped]
        public string Source { get; set; }

        [NotMapped]
        public byte[] Data { get; set; }



        #region ModificationHistory

        public DateTime DateModified { get; set; }

        public DateTime DateCreated { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [IgnoreDataMember]
        public bool IsDirty { get; set; }

        #endregion


        public string GetNameWithExtension() => $"{Name}.{Extension}";
    }

    public enum ObjType { Parent = 1, Child }
}
