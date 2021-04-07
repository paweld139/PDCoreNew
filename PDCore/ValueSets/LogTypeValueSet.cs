using PDCore.Enums;
using PDCore.Helpers.DataStructures;
using System.Collections.Generic;

namespace PDCore.ValueSets
{
    public class LogTypeValueSet : ValueSet<LogType>
    {
        public readonly static Dictionary<LogType, ValueSetItem> Dictionary = new Dictionary<LogType, ValueSetItem>
        {
            [LogType.Debug] = new ValueSetItem(Resources.Common.Debug),
            [LogType.Error] = new ValueSetItem(Resources.Common.Error),
            [LogType.Fatal] = new ValueSetItem(Resources.Common.Fatal),
            [LogType.Info] = new ValueSetItem(Resources.Common.Info),
            [LogType.Warn] = new ValueSetItem(Resources.Common.Warn)
        };

        protected override Dictionary<LogType, ValueSetItem> GetDictionary() => Dictionary;
    }
}
