namespace PDCoreNew.Models
{
    public class Email
    {
        public Email()
        {
        }

        public Email(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; set; }

        public string Body { get; set; }
    }
}
