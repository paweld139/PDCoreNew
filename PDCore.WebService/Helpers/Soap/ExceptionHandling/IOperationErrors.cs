﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCore.WebService.Helpers.Soap.ExceptionHandling
{
    public interface IOperationErrors
    {
        void HandleException(Exception ex);

        string ToString();
    }
}
