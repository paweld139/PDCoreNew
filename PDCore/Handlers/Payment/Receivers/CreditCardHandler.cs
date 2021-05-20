using PDCore.Models.Shop;
using PDCore.Models.Shop.Enums;
using PDCore.Processors.Payment;
using System.Linq;

namespace PDCore.Handlers.Payment.Receivers
{
    public class CreditCardHandler : IReceiver<Order> //PaymentHandler
    {
        private readonly CreditCardPaymentProcessor CreditCardPaymentProcessor = new CreditCardPaymentProcessor();

        public void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.CreditCard))
                CreditCardPaymentProcessor.Finalize(order);
        }
    }
}
