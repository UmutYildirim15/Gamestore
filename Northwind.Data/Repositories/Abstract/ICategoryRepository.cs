using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<IList<Product>> GetProductsByCategoryIdAsync(string id);
}