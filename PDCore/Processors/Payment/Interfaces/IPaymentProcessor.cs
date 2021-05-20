using PDCore.Models.Shop;

namespace PDCore.Processors.Payment.Interfaces
{
    public interface IPaymentProcessor
    {
        void Finalize(Order order);
    }
}
