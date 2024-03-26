
using DemoAuth.Data.Repository.IRepository;
using DemoAuth.Models;

namespace DemoAuth.Data.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext db) : base(db)
        {

        }

    }
}