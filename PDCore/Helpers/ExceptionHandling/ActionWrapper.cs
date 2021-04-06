using PDCore.Utils;
using System;
using System.Threading.Tasks;

namespace PDCore.Helpers.ExceptionHandling
{
        public static class ActionWrapper
        {
            public static Tuple<string, Exception, Task> Execute(Action action)
            {
                return DoExecuteAsync(null, action, true).Result;
            }

            public static Task<Tuple<string, Exception, Task>> ExecuteAsync(Func<Task> task)
            {
                return DoExecuteAsync(task, null, false);
            }

            private async static Task<Tuple<string, Exception, Task>> DoExecuteAsync(Func<Task> task, Action action, bool sync)
            {
                Task t = null;

                try
                {
                    if (sync)
                        action();
                    else
                    {
                        t = task();

                        await t;
                    }
                }
                catch (Exception e)
                {
                    return Tuple.Create(e.Message, e, t);
                }

                return Tuple.Create<string, Exception, Task>(WebUtils.ResultOkIndicator, null, t);
            }
        }
    }
