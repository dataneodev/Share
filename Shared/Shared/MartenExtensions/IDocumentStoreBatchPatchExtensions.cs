using System.Linq.Expressions;
using System.Security.Cryptography;
using Marten;
using Marten.Linq.MatchesSql;
using Vero.Shared.Extensions;
using Vero.Shared.Helpers;

namespace Vero.Shared.MartenExtensions
{
    public static class IDocumentStoreBatchAsyncPatchExtensions
    {
        private const int PageSize = 30;

        public static async Task BatchPatchAsync<T>(
            this IDocumentStore store,
            Expression<Func<T, bool>> find,
            Expression<Func<T, int>> idSelector,
            Func<T, Task> update
        )
            where T : notnull
        {
            if (find is null || update is null)
                throw new ArgumentException();

            await using var session = store.LightweightSession();

            var records = await GetRecordIdsAsync(find, idSelector, session);
            var count = records.Count;

            foreach (var page in GetPages(PageSize, count))
            {
                var pageRecordIds = GetPage(records, page, PageSize);
                await UpdatePageRecordsAsync(pageRecordIds, update, session);
            }
        }

        private static async Task UpdatePageRecordsAsync<T>(IEnumerable<int> pagesRecords, Func<T, Task> update, IDocumentSession session)
            where T : notnull => await RetryAction.ExecuteWithRetryAsync(
            () => UpdateRecordsAsync(pagesRecords, update, session),
            4,
            1000 + RandomNumberGenerator.GetInt32(0, 4000)
        );

        private static async Task UpdateRecordsAsync<T>(IEnumerable<int> pagesRecords, Func<T, Task> update, IDocumentSession session)
            where T : notnull
        {
            var data = await session.Query<T>()
                .Where(x => x.MatchesSql($"id = ANY(ARRAY[{string.Join(",", pagesRecords)}])"))
                .ToListAsync();

            foreach (var x in data)
            {
                await update(x);
                session.Update(x);
            }

            await session.SaveChangesAsync();

            session.EjectAllOfType(typeof(T));
            session.EjectAllPendingChanges();
        }

        private static IEnumerable<int> GetPages(int pageSize, int totalCount) =>
            totalCount == 0 ? Enumerable.Empty<int>() : Enumerable.Range(0, (int)Math.Ceiling(totalCount / (double)pageSize));

        private static IEnumerable<T> GetPage<T>(IEnumerable<T> input, int page, int pageSize) => input.Skip(page * pageSize)
            .Take(pageSize);

        private static Task<IReadOnlyList<int>> GetRecordIdsAsync<T>(
            Expression<Func<T, bool>> find,
            Expression<Func<T, int>> idSelector,
            IDocumentSession session
        )
            where T : notnull => session.Query<T>()
            .Where(find)
            .Select(idSelector)
            .OrderBy(o => o)
            .ToListAsync();
    }

    public static class IDocumentStoreBatchPatchExtensions
    {
        private const int PageSize = 30;

        /// <summary>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="store"></param>
        /// <param name="find">Query wyszukiwania rekordów do aktualizacji</param>
        /// <param name="idSelector">Selector id danego dto</param>
        /// <param name="update">Akcja aktualizująca dane dto</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task BatchPatchAsync<T>(
            this IDocumentStore store,
            Expression<Func<T, bool>> find,
            Expression<Func<T, int>> idSelector,
            Action<T> update
        )
            where T : notnull
        {
            if (find is null || update is null)
                throw new ArgumentException();

            await using var session = store.LightweightSession();

            var records = await GetRecordIdsAsync(find, idSelector, session);
            var count = records.Count;

            foreach (var page in GetPages(PageSize, count))
            {
                var pageRecordIds = GetPage(records, page, PageSize);
                await UpdatePageRecordsAsync(pageRecordIds, update, session);
            }
        }

        private static async Task UpdatePageRecordsAsync<T>(IEnumerable<int> pagesRecords, Action<T> update, IDocumentSession session)
            where T : notnull => await RetryAction.ExecuteWithRetryAsync(
            () => UpdateRecordsAsync(pagesRecords, update, session),
            4,
            1000 + RandomNumberGenerator.GetInt32(0, 4000)
        );

        private static async Task UpdateRecordsAsync<T>(IEnumerable<int> pagesRecords, Action<T> update, IDocumentSession session)
            where T : notnull
        {
            var data = await session.Query<T>()
                .Where(x => x.MatchesSql($"id = ANY(ARRAY[{string.Join(",", pagesRecords)}])"))
                .ToListAsync();

            data.ForEach(
                x =>
                {
                    update(x);
                    session.Update(x);
                }
            );

            await session.SaveChangesAsync();

            session.EjectAllOfType(typeof(T));
            session.EjectAllPendingChanges();
        }

        private static IEnumerable<int> GetPages(int pageSize, int totalCount) =>
            totalCount == 0 ? Enumerable.Empty<int>() : Enumerable.Range(0, (int)Math.Ceiling(totalCount / (double)pageSize));

        private static IEnumerable<T> GetPage<T>(IEnumerable<T> input, int page, int pageSize) => input.Skip(page * pageSize)
            .Take(pageSize);

        private static Task<IReadOnlyList<int>> GetRecordIdsAsync<T>(
            Expression<Func<T, bool>> find,
            Expression<Func<T, int>> idSelector,
            IDocumentSession session
        )
            where T : notnull => session.Query<T>()
            .Where(find)
            .Select(idSelector)
            .OrderBy(o => o)
            .ToListAsync();
    }
}