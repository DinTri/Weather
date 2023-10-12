namespace Weather.Api.Data
{
    public interface IDapperContextQuery
    {
        IEnumerable<T> Query<T>(string sql, object? parameters = null);
    }

}
