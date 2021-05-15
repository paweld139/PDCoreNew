namespace PDCore.Models.Search
{
    public abstract class SearchRequest
    {
        public bool CountRows { get; set; }

        public int PageSize { get; set; }

        public int CurentPage { get; set; }
    }
}
