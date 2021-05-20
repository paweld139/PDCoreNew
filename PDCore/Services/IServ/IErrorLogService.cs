using System;

namespace PDCore.Services.IServ
{
    public interface IErrorLogService
    {
        string Log(Exception exception);
    }
}
