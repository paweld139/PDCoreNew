using PDCore.Models.Shop;
using PDCore.Models.Shop.Enums;
using PDCore.Processors.Payment;
using System.Linq;

namespace PDCore.Handlers.Payment.Receivers
{
    public class InvoiceHandler : IReceiver<Order> //PaymentHandler
    {
        private readonly InvoicePaymentProcessor InvoicePaymentProcessor = new InvoicePaymentProcessor();

        public void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.Invoice))
                InvoicePaymentProcessor.Finalize(order);
        }
    }
}
