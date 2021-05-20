using PDCore.Models.Shop.Enums;

namespace PDCore.Models.Shop
{
    public class Payment
    {
        public PaymentProvider PaymentProvider { get; set; }

        public decimal Amount { get; set; }
    }
}
