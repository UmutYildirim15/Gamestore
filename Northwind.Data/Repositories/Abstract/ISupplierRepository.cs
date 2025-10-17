using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract;

public interface ISupplierRepository : IGenericRepository<Supplier>
{
    Task<IList<Product>> GetProductsBySupplierIdAsync(string id);
}