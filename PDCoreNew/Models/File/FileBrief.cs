using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace PDCoreNew.Models
{
    [DataContract(Name = "file", Namespace = "")]
    public class FileBrief
    {
        [Display(Name = nameof(Id))]
        public int Id { get; set; }

        [LocalizedRequired]
        [Display(Name = nameof(Name))]
        [DataType(DataType.Text)]
        [LocalizedStringLengthMinMax(1, 100)]
        public string Name { get; set; }

        [DataType(DataType.Text)]
        [LocalizedStringLengthMinMax(1, 20)]
        public string Extension { get; set; }

        public string UserId { get; set; }
    }
}
