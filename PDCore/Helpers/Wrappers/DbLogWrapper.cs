﻿using PDCore.Interfaces;
using System;

namespace PDCore.Helpers.Wrappers
{
    public static class DbLogWrapper
    {
        private static readonly Lazy<StopWatchWrapper> stopwatchWrapper = new Lazy<StopWatchWrapper>();

        private static void Log(TimeSpan timeSpan, string query, string connectionString, ILogger logger)
        {
            string message = string.Format("{5}GetDataTable [{0}][{1}]{2} {3} [{4} ms]{5}",
                        DateTime.Now, connectionString, string.Empty/*Environment.NewLine + Environment.StackTrace*/,
                        (Environment.NewLine + query), timeSpan.TotalMilliseconds, Environment.NewLine);

            logger.Info(message);
        }

        public static T Execute<T>(Func<string, T> func, string query, string connectionString, ILogger logger, bool loggingEnabled)
        {
            if (!loggingEnabled)
                return func(query);

            var time = stopwatchWrapper.Value.Measure(func, query);

            Log(time.Item1, query, connectionString, logger);

            return time.Item2;
        }
    }
}
