using Microsoft.Data.Sqlite;

namespace DemoAuth.Data.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqliteParameter> sqlParameters, bool tracked = false);
        Task<int> ExecuteSqlAsync(string sql, List<SqliteParameter> sqlParameters);
        Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqliteParameter> sqlParameters);
    }
}