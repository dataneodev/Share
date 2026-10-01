using Dapper;
using Npgsql;
using Vero.Shared.DDD;

namespace Vero.Shared.EntityFramework
{
    public interface IIdGeneratorBase<TEntityId>
        where TEntityId : EntityId
    {
        Task<TEntityId> NextAsync();

        Task<IReadOnlyList<TEntityId>> NextAsync(int count);
    }

    public abstract class IdGeneratorBase
    {
        private readonly string _connectionString;
        private readonly string _tableName;

        protected IdGeneratorBase(DatabaseOptions options, string tableName)
        {
            _connectionString = options.ConnectionString;
            _tableName = tableName;
        }

        protected async Task<IReadOnlyList<int>> NextAsync(int count)
        {
            if (count <= 0)
                return Array.Empty<int>();

            await using var conn = new NpgsqlConnection(_connectionString);
            return (await conn.QueryAsync<int>($"SELECT nextval(:{nameof(_tableName)}) FROM generate_series(1, :{nameof(count)}) n", new { _tableName, count }))
                .AsList();
        }

        protected async Task<int> NextAsync()
        {
            await using var conn = new NpgsqlConnection(_connectionString);
            return await conn.QuerySingleAsync<int>($"SELECT NEXTVAL(:{nameof(_tableName)})", new { _tableName });
        }
    }

    public abstract class IdGeneratorBase<TEntityId> : IdGeneratorBase
        where TEntityId : EntityId
    {
        protected IdGeneratorBase(DatabaseOptions options, string tableName) : base(options, tableName)
        {
        }

        public new async Task<IReadOnlyList<TEntityId>> NextAsync(int count)
        {
            var type = typeof(TEntityId);
            var ids = await base.NextAsync(count);
            return ids.Select(id => Activator.CreateInstance(type, id))
                .Cast<TEntityId>()
                .ToArray();
        }

        public new async Task<TEntityId> NextAsync()
        {
            var type = typeof(TEntityId);
            var id = await base.NextAsync();
            return (Activator.CreateInstance(type, id) as TEntityId)!;
        }
    }
}