using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Vayosoft.Data.Dapper.MySQL
{
    public class DbConnection : IDisposable
    {
        private readonly IDbConnection connection;
        public DbConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            connection = new MySqlConnection(connectionString);
        }

        public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return (await connection.ExecuteAsync(sql, param, transaction));
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return (await connection.QueryAsync<T>(sql, param, transaction)).AsList();
            //return (await connection.QueryAsync<T>(
            //    new CommandDefinition(sql, param, transaction, cancellationToken: cancellationToken)
            //)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
        {
            return await connection.QuerySingleAsync<T>(sql, param, transaction);
        }

        public void Dispose()
        {
            connection.Dispose();
        }
    }
}