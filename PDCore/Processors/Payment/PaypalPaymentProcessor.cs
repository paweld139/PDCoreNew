﻿using PDCore.Models.Shop;
using PDCore.Models.Shop.Enums;
using PDCore.Processors.Payment.Interfaces;
using System.Linq;

namespace PDCore.Processors.Payment
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
