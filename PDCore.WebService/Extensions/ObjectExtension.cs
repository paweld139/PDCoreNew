using PDCore.Helpers.Wrappers.DisposableWrapper;
using PDCore.WebService.Helpers.Soap.ExceptionHandling;
using System.ServiceModel;

namespace PDCore.WebService.Extensions
{
    public static class ObjectExtension
    {
        // specific handling for service-model
        public static IDisposableWrapper<TProxy> Wrap<TProxy, TService>(
            this TProxy proxy)
            where TProxy : ClientBase<TService>
            where TService : class
        {
            return new ClientWrapper<TProxy, TService>(proxy);
        }
    }
}
