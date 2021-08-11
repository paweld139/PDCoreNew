using PDCoreNew.Interfaces;
using System.Runtime.Serialization;

namespace PDCoreNew.Models
{
    [DataContract(Name = "rowInfo")]
    public class RowInfo
    {
        public RowInfo(IModificationHistory modificationHistory)
        {
            RowVersion = modificationHistory.RowVersion;
        }

        [DataMember(Name = "rowVersion")]
        public byte[] RowVersion { get; set; }
    }
}
