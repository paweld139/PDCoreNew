using PDCoreNew.Entities.Details;
using PDCoreNew.Enums;
using PDCoreNew.Utils;
using PDCoreNew.ValueSets;

namespace PDCoreNew.Lazy.Proxies
{
    public class LogDetailsProxy : LogDetails
    {
        public override string LogTypeName => EnumUtils.GetTranslated<LogTypeValueSet, LogType>(LogLevel);
    }
}
