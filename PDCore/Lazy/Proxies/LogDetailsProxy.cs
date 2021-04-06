using PDCore.Entities.Details;
using PDCore.Enums;
using PDCore.Utils;
using PDCore.ValueSets;

namespace PDCore.Lazy.Proxies
{
    public class LogDetailsProxy : LogDetails
    {
        public override string LogTypeName => EnumUtils.GetTranslated<LogTypeValueSet, LogType>(LogLevel);
    }
}
