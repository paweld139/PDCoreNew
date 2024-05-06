using IronRuby;
using PDCore.Extensions;
using System.Collections.Generic;

namespace PDCore.Common.Utils
{
    public static class ReflectionUtils
    {
        public static dynamic GetIronRubyRunitimeGlobals(IDictionary<string, object> variables, string fileToExecute)
        {
            var engine = Ruby.CreateEngine();

            var scope = engine.CreateScope();

            variables?.ForEach(v => scope.SetVariable(v.Key, v.Value));

            if (!string.IsNullOrEmpty(fileToExecute))
                engine.ExecuteFile(fileToExecute, scope);

            return engine.Runtime.Globals;
        }
    }
}
