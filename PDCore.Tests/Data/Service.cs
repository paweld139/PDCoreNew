namespace PDCore.Tests.Data
{
    public interface ILogger
    {

    }

    public class SqlServerLogger : ILogger
    {

    }

    public interface IRepository<T>
    {

    }

    public class SqlRepository<T> : IRepository<T>
    {
        public SqlRepository(ILogger logger)
        {
            _ = logger;
        }
    }

    public class Customer
    {

    }

    public class InvoiceService
    {
        public InvoiceService(IRepository<Customer> repository, ILogger logger)
        {
            _ = repository;

            _ = logger;
        }
    }
}
