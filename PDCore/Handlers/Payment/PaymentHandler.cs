﻿using PDCore.Models.Shop;
using PDCore.Models.Shop.Enums;
using PDCore.Models.Shop.Exceptions;

namespace PDCore.Handlers.Payment
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
