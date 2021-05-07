using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace PDCore.Helpers.PythonExecute
{
    public class PythonScript
    {
        private readonly ScriptEngine _engine;

        public PythonScript()
        {
            _engine = Python.CreateEngine();
        }

        public TResult RunFromFile<TResult>(string filePath, string variableName)
        {
            var scriptSource = _engine.CreateScriptSourceFromFile(filePath);

            return Run<TResult>(scriptSource, variableName);
        }

        public TResult RunFromString<TResult>(string code, string variableName)
        {
            var scriptSource = _engine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);

            return Run<TResult>(scriptSource, variableName);
        }

        private TResult Run<TResult>(ScriptSource source, string variableName)
        {
            CompiledCode cc = source.Compile();

            ScriptScope scope = _engine.CreateScope();

            cc.Execute(scope);

            return scope.GetVariable<TResult>(variableName);
        }
    }
}
