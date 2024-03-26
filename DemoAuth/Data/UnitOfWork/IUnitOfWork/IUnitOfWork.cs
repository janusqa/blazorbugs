using DemoAuth.Data.Repository.IRepository;
using Microsoft.EntityFrameworkCore.Storage;

namespace DemoAuth.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; init; }
        ICategoryRepository Categories { get; init; }


        IDbContextTransaction Transaction();
    }
}