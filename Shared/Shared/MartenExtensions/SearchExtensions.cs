using System.Linq.Expressions;
using System.Text;
using Marten.Linq.MatchesSql;

namespace Vero.Shared.MartenExtensions
{
    public static class SearchExtensions
    {
        public static string CreateSearchTemplate<T>(params Expression<Func<T, object>>[] expressions)
            where T : class
        {
            var props = Property.GetFullNames(expressions);
            var builder = new StringBuilder("( ");

            for (var i = 0; i < props.Length; i++)
            {
                if (i > 0)
                    builder.Append(" OR ");

                var selector = string.Join(
                    "->",
                    props[i]
                        .Split(".")
                        .Select(p => $"'{p}'")
                );

                builder.Append($"(data->{selector})::text ILIKE ?");
            }

            builder.Append(" )");

            return builder.ToString();
        }

        public static IQueryable<T> SearchCustom<T>(this IQueryable<T> query, string search, string template)
            where T : class
        {
            var parameters = Enumerable.Range(0, template.Count(c => c == '?'))
                .Select(_ => $"%{search}%".ToLower() as object)
                .ToArray();

            return query.Where(x => x.MatchesSql(template, parameters));
        }

        public static string CreateSqlOrderQueryPart(string? sortOrder, bool useNullOrder, params string[] paths)
        {
            var sql = $"{CreateJsonbPath(paths)} {sortOrder}";

            if (useNullOrder)
            {
                sql += GetNullSort(sortOrder);
            }

            return sql;
        }

        public static string CreateJsonbPath(params string[] paths) => $"data->{string.Join("->", paths.Select(s => $"'{s}'"))}";

        private static string GetNullSort(string? sortOrder) => sortOrder == SortOrder.Asc.Value ? " NULLS FIRST" : " NULLS LAST";
    }
}