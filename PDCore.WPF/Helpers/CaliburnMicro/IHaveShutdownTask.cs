using Caliburn.Micro;

namespace PDCore.WPF.Helpers.CaliburnMicro
{
    public interface IHaveShutdownTask
    {
        IResult GetShutdownTask();
    }
}
