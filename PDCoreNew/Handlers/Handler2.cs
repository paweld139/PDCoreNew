using System.Collections.Generic;

namespace PDCoreNew.Handlers
{
    public class Handler2<T> where T : class
    {
        protected readonly IList<IReceiver<T>> receivers;

        public Handler2(params IReceiver<T>[] receivers)
        {
            this.receivers = receivers;
        }

        public virtual void Handle(T request)
        {
            foreach (var receiver in receivers)
            {
                receiver.Handle(request);
            }
        }
    }
}
