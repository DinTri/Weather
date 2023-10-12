using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System.Data;

namespace Weather.Api.Data
{
    public class DapperContext : IDapperContext
    {
        private ConnectionStringOptions connectionStringOptions;
        public DapperContext(IOptionsMonitor<ConnectionStringOptions> optionsMonitor)
        {
            connectionStringOptions = optionsMonitor.CurrentValue;
        }
        public IDbConnection CreateConnection() => new SqlConnection(connectionStringOptions.TrifonSqlConnection);

        public IEnumerable<T> Query<T>(string sql, object? parameters = null)
        {
            using var connection = CreateConnection();
            return connection.Query<T>(sql, parameters);
        }

    }

}
