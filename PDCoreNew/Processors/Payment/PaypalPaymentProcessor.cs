using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Enums;
using PDCoreNew.Processors.Payment.Interfaces;
using System.Linq;

namespace PDCoreNew.Processors.Payment
{
    public class PaypalPaymentProcessor : IPaymentProcessor
    {
        public void Finalize(Order order)
        {
            var payment = order.SelectedPayments.FirstOrDefault(p => p.PaymentProvider == PaymentProvider.Paypal);

            if (payment == null) return;

            order.FinalizedPayments.Add(payment);
        }
    }
}
