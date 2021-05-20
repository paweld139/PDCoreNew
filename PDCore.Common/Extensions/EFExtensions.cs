using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PDCore.Common.Extensions
{
    public static class EFExtensions
    {
        public static Task<int> CountSafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!(source is IDbAsyncEnumerable<TSource>))
                return Task.FromResult(source.Count());

            return source.CountAsync();
        }

        public static Task<bool> AnySafe<TSource>(this IQueryable<TSource> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!(source is IDbAsyncEnumerable<TSource>))
                return Task.FromResult(source.Any());

            return source.AnyAsync();
        }

        public static Task<long> SumSafe<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, long>> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (!(source is IDbAsyncEnumerable<TSource>))
                return Task.FromResult(source.Sum(selector));

            return source.SumAsync(selector);
        }
    }
}
