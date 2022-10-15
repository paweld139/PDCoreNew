using System;

namespace PDCoreNew.Contracts
{
    public interface IReceiversFactory<TSheetModel> : IFactory<IReceiverAsync<TSheetModel>[]>
        where TSheetModel : class
    {
        IReceiverAsync<TSheetModel>[] Create(Action<string, string> addError);
    }
}
