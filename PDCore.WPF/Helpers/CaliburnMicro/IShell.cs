using Caliburn.Micro;

namespace PDCore.WPF.Helpers.CaliburnMicro
{
    public interface IShell : IConductor, IGuardClose
    {
        IDialogManager Dialogs { get; }
    }
}
