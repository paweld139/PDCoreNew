namespace PDCoreNew.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorStatusCode { get; set; }

        public string OriginalURL { get; set; }


        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

        public bool ShowErrorStatusCode => !string.IsNullOrEmpty(ErrorStatusCode);

        public bool ShowOriginalURL => !string.IsNullOrEmpty(OriginalURL);
    }
}
