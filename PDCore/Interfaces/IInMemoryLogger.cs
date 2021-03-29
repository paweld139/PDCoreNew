using PDCore.Helpers.DataStructures.Buffer;

namespace PDCore.Interfaces
{
    public interface IInMemoryLogger : ILogger
    {
        IBuffer<string> Logs { get; }
    }
}
