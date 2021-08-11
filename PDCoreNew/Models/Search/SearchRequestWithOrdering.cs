namespace PDCoreNew.Models.Search
{
    public abstract class SearchRequestWithOrdering : SearchRequest
    {
        public string OrderDirection { get; set; }
    }
}
