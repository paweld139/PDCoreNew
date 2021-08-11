using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Enums;
using PDCoreNew.Processors.Payment;
using System.Linq;

namespace PDCoreNew.Handlers.Payment.Receivers
{
    public class CreditCardHandler : IReceiver<Order> //PaymentHandler
    {
        private readonly CreditCardPaymentProcessor CreditCardPaymentProcessor = new();

        public void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.CreditCard))
                CreditCardPaymentProcessor.Finalize(order);
        }
    }
}
