﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PDCore.WPF.Helpers.CaliburnMicro
{
    public interface IDocumentWorkspace : IWorkspace
    {
        void Edit(object document);
    }
}