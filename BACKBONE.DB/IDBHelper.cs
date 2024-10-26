using System.Data;


namespace BACKBONE.DB
{
    public interface IDBHelper
    {
        IEnumerable<T> Query<T>(string sql, object? parameters = null, CommandType? commandType = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CommandType? commandType = null);
        T QueryFirstOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null);
        Task<T> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CommandType? commandType = null);
        T QuerySingleOrDefault<T>(string sql, object? parameters = null, CommandType? commandType = null);

        int Execute(string sql, object? parameters = null, CommandType? commandType = null);
        Task<int> ExecuteAsync(string sql, object? parameters = null, CommandType? commandType = null);
        T Insert<T>(string sql, object? parameters = null, CommandType? commandType = null);
        int ExecuteTransaction(IEnumerable<SqlCommandInfo> commands);
        Task<int> ExecuteTransactionAsync(List<SqlCommandInfo> commands);
        IEnumerable<IDictionary<string, object>> ExecuteStoredProcedure(string storedProcedureName, object? parameters = null);
        int ExecuteScalar(string sql, object? parameters = null, CommandType? commandType = null);
        IEnumerable<IEnumerable<T>> QueryMultiple<T>(string sql, object? parameters = null, CommandType? commandType = null);
        DataTable ConvertCollectionToDataTable<T>(IEnumerable<T> collection);

    }
}
