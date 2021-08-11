using System.Collections.Generic;

namespace PDCoreNew.Helpers.DataStructures.Buffer
{
    public interface IBuffer<T> : IEnumerable<T>
    {
        bool IsEmpty { get; }

        void Write(T value);

        T Read();
    }
}
