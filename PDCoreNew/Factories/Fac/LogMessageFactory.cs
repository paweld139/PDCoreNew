using PDCoreNew.Enums;
using PDCoreNew.Extensions;
using PDCoreNew.Factories.IFac;
using System;
using System.Linq;
using System.Text;

namespace PDCoreNew.Factories.Fac
{
    public class LogMessageFactory : ILogMessageFactory
    {
        public string Create(string message, Exception exception, LogType logType)
        {
            StringBuilder result = new();

            string date = DateTime.Now.ToDMY();

            result.AppendFormat("[{1}] {0}: ", logType, date);

            var items = new[] { message, exception?.ToString() };

            string itemsText = string.Join(", Wyjątek: ", items.Where(x => !string.IsNullOrEmpty(x)));

            result.Append(itemsText);


            return result.ToString();
        }
    }
}
