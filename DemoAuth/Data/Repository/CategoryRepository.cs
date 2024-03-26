
using DemoAuth.Data.Repository.IRepository;
using DemoAuth.Models;

namespace DemoAuth.Data.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext db) : base(db)
        {

        }

    }
}