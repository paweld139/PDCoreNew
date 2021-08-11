namespace PDCoreNew.Builders.Account
{
    public interface IAccountBuilder : IBuilder<Account>
    {
        AccountBuilder WithLatePaymentStatus();

        AccountBuilder WithVipCustomer();
    }
}
