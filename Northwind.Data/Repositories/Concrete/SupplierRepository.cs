using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Repositories.Concrete;

public class SupplierRepository(IMongoContext context) : GenericRepository<Supplier>(context), ISupplierRepository
{
    public async Task<IList<Product>> GetProductsBySupplierIdAsync(string id)
    {
        var products = Context.GetCollection<Product>().AsQueryable();
        var queryResult = await DbSet.AsQueryable()
            .Where(s => s.Id == id)
            .GroupJoin(
                products,
                s => s.SupplierId,
                p => p.SupplierId,
                (s, p) => p.ToList())
            .SingleAsync();
        return queryResult;
    }
}