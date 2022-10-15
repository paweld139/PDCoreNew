namespace PDCoreNew.Models.Results
{
    public class ConfirmResult
    {
        public ConfirmResult(bool isConfirm, string message, string yes, string no, string title)
        {
            IsConfirm = isConfirm;
            Message = message;
            Yes = yes;
            No = no;
            Title = title;
        }

        public bool IsConfirm { get; set; }

        public string Message { get; set; }

        public string Yes { get; set; }

        public string No { get; set; }

        public string Title { get; set; }
    }
}
