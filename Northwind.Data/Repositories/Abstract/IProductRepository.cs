using Northwind.Data.Entities;
using Shared.Models;

namespace Northwind.Data.Repositories.Abstract;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<EntityFilter<Product>> GetFilteredProductsAsync(GameFilter filter);

    Task<IList<Category>> GetCategoriesByProductKeyAsync(string key);

    Task<Supplier> GetSupplierByProductKeyAsync(string key);
}