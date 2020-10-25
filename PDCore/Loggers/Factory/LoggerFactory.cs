using PDCore.Factories.Fac;
using PDCore.Factories.IFac;
using PDCore.Helpers;
using PDCore.Interfaces;

namespace PDCore.Loggers.Factory
{
    public class LoggerFactory : Factory<Enums.Loggers, ILogger>
    {
        protected override string ElementsNamespace => typeof(Logger).Namespace;

        protected override string ElementsPostfix => "Logger";

        protected override void ConfigureContainer(Container container)
        {
            container.For<ILogMessageFactory>().Use<LogMessageFactory>();
        }

        //public static LoggerFactory InitializeLoggers() => new LoggerFactory(); //W przypadku gdy konstrukcja obiektu jest skomplikowana
    }
}
