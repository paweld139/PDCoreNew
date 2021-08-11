using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Enums;
using PDCoreNew.Processors.Payment;
using System.Linq;

namespace PDCoreNew.Handlers.Payment.Receivers
{
    public class InvoiceHandler : IReceiver<Order> //PaymentHandler
    {
        private readonly InvoicePaymentProcessor InvoicePaymentProcessor = new();

        public void Handle(Order order)
        {
            if (order.SelectedPayments.Any(x => x.PaymentProvider == PaymentProvider.Invoice))
                InvoicePaymentProcessor.Finalize(order);
        }
    }
}
