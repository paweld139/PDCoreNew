using System;

namespace PDCore.WebService.Helpers.Soap.ExceptionHandling
{
    public interface IOperationErrors
    {
        void HandleException(Exception ex);

        string ToString();
    }
}
