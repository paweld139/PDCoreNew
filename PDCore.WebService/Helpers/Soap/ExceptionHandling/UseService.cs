using System.ServiceModel;

namespace PDCore.WebService.Helpers.Soap.ExceptionHandling
{
    public delegate void UseServiceDelegate<T>(T proxy);

    public static class Service<T>
    {
        public static ChannelFactory<T> _channelFactory = new ChannelFactory<T>("");

        public static void Use(UseServiceDelegate<T> codeBlock)
        {
            IClientChannel proxy = (IClientChannel)_channelFactory.CreateChannel();

            bool success = false;

            try
            {
                codeBlock((T)proxy);

                proxy.Close();

                success = true;
            }
            finally
            {
                if (!success)
                {
                    proxy.Abort();
                }
            }
        }
    }
}
