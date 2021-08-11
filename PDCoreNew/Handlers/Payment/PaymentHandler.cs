using PDCoreNew.Models.Shop;
using PDCoreNew.Models.Shop.Enums;
using PDCoreNew.Models.Shop.Exceptions;

namespace PDCoreNew.Handlers.Payment
{
    public class PaymentHandler : Handler<Order>
    {
        public override void Handle(Order order)
        {
            if (Next == null && order.AmountDue > 0)
            {
                throw new InsufficientPaymentException();
            }

            if (order.AmountDue > 0)
            {
                base.Handle(order);
            }
            else
            {
                order.ShippingStatus = ShippingStatus.ReadyForShipment;
            }
        }
    }
}
