using PDCoreNew.Entities.Briefs;
using System.ComponentModel.DataAnnotations;

namespace PDCoreNew.Entities.Basic
{
    public class DictionaryBasic : DictionaryBrief
    {
        [LocalizedRequired]
        public string Name { get; set; }
    }
}
