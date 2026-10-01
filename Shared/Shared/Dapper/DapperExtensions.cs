using System.Data;
using Dapper;

namespace Vero.Shared.Dapper
{
    public static class DapperExtensions
    {
        private const int ConnectionTimeOut = 60;
        private const int RetryCount = 3;

        public static async Task<IEnumerable<T>> QueryAsyncWithRetry<T>(this IDbConnection cnn, string sql, object? param = null)
        {
            var retryCount = 0;

            while (true)
            {
                try
                {
                    return await cnn.QueryAsync<T>(sql, param, null, ConnectionTimeOut);
                }
                catch (Exception)
                {
                    if (retryCount > RetryCount)
                        throw;

                    retryCount++;
                    await Task.Delay(3000 * retryCount);
                }
            }
        }
    }
}