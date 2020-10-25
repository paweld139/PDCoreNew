using Caliburn.Micro;

namespace PDCore.WPF.Helpers.WPF.CaliburnMicro
{
    public interface IShell : IConductor, IGuardClose
    {
        IDialogManager Dialogs { get; }
    }
}
