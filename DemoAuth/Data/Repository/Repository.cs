using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using DemoAuth.Data.Repository.IRepository;

namespace DemoAuth.Data.Repository
{

    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet = _db.Set<T>();
        }

        public async Task<IEnumerable<T>> FromSqlAsync(string sql, List<SqliteParameter> sqlParameters, bool tracked)
        {
            return tracked
                ? await dbSet.FromSqlRaw(sql, sqlParameters.ToArray()).ToListAsync()
                : await dbSet.FromSqlRaw(sql, sqlParameters.ToArray()).AsNoTracking().ToListAsync();
        }

        public async Task<int> ExecuteSqlAsync(string sql, List<SqliteParameter> sqlParameters)
        {
            return await _db.Database.ExecuteSqlRawAsync(sql, sqlParameters.ToArray());
        }

        public async Task<IEnumerable<U>> SqlQueryAsync<U>(string sql, List<SqliteParameter> sqlParameters)
        {
            return await _db.Database.SqlQueryRaw<U>(sql, sqlParameters.ToArray()).ToListAsync();
        }
    }
}