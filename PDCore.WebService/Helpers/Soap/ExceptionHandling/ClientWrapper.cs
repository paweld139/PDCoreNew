using PDCore.Helpers.Wrappers.DisposableWrapper;
using System.ServiceModel;

namespace PDCore.WebService.Helpers.Soap.ExceptionHandling
{
    public class ClientWrapper<TProxy, TService> : DisposableWrapper<TProxy> where TProxy : ClientBase<TService> where TService : class
    {
        public ClientWrapper(TProxy proxy) : base(proxy) { }

        protected override void OnDispose()
        {
            // lots of code per state of BaseObject
        }
    }
}
