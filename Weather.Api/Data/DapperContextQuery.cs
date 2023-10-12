namespace Weather.Api.Data
{
    public class DapperContextQuery : IDapperContextQuery
    {
        private readonly DapperContext _dapperContext;

        public DapperContextQuery(DapperContext dapperContext)
        {
            _dapperContext = dapperContext;
        }

        public IEnumerable<T> Query<T>(string sql, object parameters = null)
        {
            // Delegate the call to the actual DapperContext.Query method
            return _dapperContext.Query<T>(sql, parameters);
        }
    }
}
