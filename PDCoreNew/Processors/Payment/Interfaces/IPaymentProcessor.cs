using PDCoreNew.Models.Shop;

namespace PDCoreNew.Processors.Payment.Interfaces
{
    public interface IPaymentProcessor
    {
        void Finalize(Order order);
    }
}
