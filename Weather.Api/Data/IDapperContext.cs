using System.Data;

namespace Weather.Api.Data
{
    public interface IDapperContext
    {
        IDbConnection CreateConnection();
        IEnumerable<T> Query<T>(string sql, object parameters = null);

    }
}
