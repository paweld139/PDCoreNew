using PDCoreNew.Models.Shop.Enums;

namespace PDCoreNew.Models.Shop
{
    public class Payment
    {
        public PaymentProvider PaymentProvider { get; set; }

        public decimal Amount { get; set; }
    }
}
