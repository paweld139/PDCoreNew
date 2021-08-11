using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Enums;
using PDCoreNew.Processors.Payment;
using System.Linq;

namespace PDCoreNew.Handlers.Payment.Receivers
{
    public class PaypalHandler : IReceiver<Order> //PaymentHandler
    {
        private readonly PaypalPaymentProcessor PaypalPaymentProcessor = new();

        public void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.Paypal))
                PaypalPaymentProcessor.Finalize(order);
        }
    }
}
