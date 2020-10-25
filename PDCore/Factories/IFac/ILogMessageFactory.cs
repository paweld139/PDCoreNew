﻿using PDCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDCore.Factories.IFac
{
    public interface ILogMessageFactory
    {
        string Create(string message, Exception exception, LogType logType);
    }
}
