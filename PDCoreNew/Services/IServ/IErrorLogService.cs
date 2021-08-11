using System;

namespace PDCoreNew.Services.IServ
{
    public interface IErrorLogService
    {
        string Log(Exception exception);
    }
}
