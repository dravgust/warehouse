using System.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace Vayosoft.Dapper.MySQL
{
    public class DbConnection : IDisposable
    {
        public DbConnection(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            InnerConnection = new MySqlConnection(connectionString);
        }

        protected IDbConnection InnerConnection { get; }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await InnerConnection.ExecuteAsync(sql, param, transaction));
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return (await InnerConnection.QueryAsync<T>(sql, param, transaction)).AsList();
            //return (await connection.QueryAsync<T>(
            //    new CommandDefinition(sql, param, transaction, cancellationToken: cancellationToken)
            //)).AsList();
        }

        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await InnerConnection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, CancellationToken cancellationToken = default)
        {
            return await InnerConnection.QuerySingleAsync<T>(sql, param, transaction);
        }

        public void Dispose()
        {
            InnerConnection.Dispose();
        }
    }
}