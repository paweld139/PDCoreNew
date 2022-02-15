using System.ComponentModel.DataAnnotations;

namespace PDCoreNew.Entities.Briefs
{
    public class DictionaryBrief
    {
        [LocalizedRequired]
        public string Key { get; set; }

        [LocalizedRequired]
        public string Value { get; set; }
    }
}
