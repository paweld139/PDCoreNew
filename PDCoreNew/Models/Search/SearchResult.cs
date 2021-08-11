using System.Collections.Generic;

namespace PDCoreNew.Models
{
    public class Result<T>
    {
        public Result(ICollection<T> input)
        {
            Results = input;
        }

        public ICollection<T> Results { get; set; }

        public static implicit operator Result<T>(List<T> input) => new(input);
    }

    public class SearchResult<T>
    {
        public SearchResult(int? pages, IEnumerable<T> rows)
        {
            Pages = pages;
            Rows = rows;
        }

        public int? Pages { get; private set; }
        public IEnumerable<T> Rows { get; private set; }
    }

    public class SearchResult<T, U>
    {
        public SearchResult(int? pages, IEnumerable<T> rows1, IEnumerable<U> rows2)
        {
            Pages = pages;
            Rows1 = rows1;
            Rows2 = rows2;
        }

        public int? Pages { get; private set; }
        public IEnumerable<T> Rows1 { get; private set; }
        public IEnumerable<U> Rows2 { get; private set; }
    }

    public class SearchResult<T, U, W>
    {
        public SearchResult(int? pages, IEnumerable<T> rows1, IEnumerable<U> rows2, IEnumerable<W> rows3)
        {
            Pages = pages;
            Rows1 = rows1;
            Rows2 = rows2;
            Rows3 = rows3;
        }

        public int? Pages { get; private set; }
        public IEnumerable<T> Rows1 { get; private set; }
        public IEnumerable<U> Rows2 { get; private set; }
        public IEnumerable<W> Rows3 { get; private set; }
    }
}
