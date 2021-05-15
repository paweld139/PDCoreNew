using System.Collections;
using System.Collections.Generic;

namespace PDCore.Helpers.DataStructures.Buffer
{
    public class Buffer<T> : IBuffer<T>
    {
        protected Queue<T> queue = new Queue<T>();

        public virtual bool IsEmpty => queue.Count == 0;

        public virtual void Write(T value) => queue.Enqueue(value);

        public virtual T Read() => queue.Dequeue();

        public IEnumerator<T> GetEnumerator() => queue.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
