using DemoAuth.Data.Repository;
using DemoAuth.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoAuth.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public IProductRepository Products { get; init; }
        public ICategoryRepository Categories { get; init; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            Products = new ProductRepository(_db);
            Categories = new CategoryRepository(_db);
        }

        public void Dispose()
        {
            _db.Dispose();
        }

        public IDbContextTransaction Transaction() => _db.Database.BeginTransaction();
    }
}