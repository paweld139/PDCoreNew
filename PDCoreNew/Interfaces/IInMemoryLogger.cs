using PDCoreNew.Helpers.DataStructures.Buffer;

namespace PDCoreNew.Interfaces
{
    public interface IInMemoryLogger : ILogger
    {
        IBuffer<string> Logs { get; }
    }
}
