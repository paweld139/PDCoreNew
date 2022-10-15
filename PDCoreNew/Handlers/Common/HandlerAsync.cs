using PDCoreNew.Contracts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PDCoreNew.Handlers.Common
{
    public class HandlerAsync<T> : Handler2<T> where T : class
    {
        protected readonly IList<IReceiverAsync<T>> receiversAsync;

        public HandlerAsync(params IReceiverAsync<T>[] receiversAsync)
        {
            this.receiversAsync = receiversAsync;
        }

        public virtual async ValueTask HandleAsync(T request)
        {
            foreach (var receiverAsync in receiversAsync)
            {
                await receiverAsync.HandleAsync(request);
            }
        }
    }
}
