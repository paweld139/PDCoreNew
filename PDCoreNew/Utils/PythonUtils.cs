﻿using System;
using System.Diagnostics;
using System.IO;

namespace PDCoreNew.Utils
{
    public  static class PythonUtils
    {
        public static string Run(string cmd, string args = "")
        {
            ProcessStartInfo start = new()
            {
                FileName = "python",
                Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args),
                UseShellExecute = false,// Do not use OS shell
                CreateNoWindow = true, // We don't need new window
                RedirectStandardOutput = true,// Any output, generated by application will be redirected back
                RedirectStandardError = true // Any error in standard output will be redirected back (for example exceptions)
            };

            using Process process = Process.Start(start);

            using StreamReader reader = process.StandardOutput;

            string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script

            if (!string.IsNullOrWhiteSpace(stderr))
                throw new Exception(stderr);

            string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")

            return result;
        }
    }
}