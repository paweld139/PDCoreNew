namespace PDCoreNew.Models.Results
{
    public class MessageResult<T>
    {
        public MessageResult(T message)
        {
            Message = message;
        }

        public T Message { get; set; }
    }
}
