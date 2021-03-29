using System;
using System.Net;

namespace PDCore.Helpers
{
    public class WebClientWithTimeout : WebClient
    {
        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest webRequest = base.GetWebRequest(address);

            webRequest.Timeout = 20 * 60 * 1000;

            return webRequest;
        }
    }
}
