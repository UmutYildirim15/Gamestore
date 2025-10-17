using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Repositories.Concrete;

public class CategoryRepository(IMongoContext context) : GenericRepository<Category>(context), ICategoryRepository
{
    public async Task<IList<Product>> GetProductsByCategoryIdAsync(string id)
    {
        var products = Context.GetCollection<Product>().AsQueryable();
        var queryResult = await DbSet.AsQueryable()
            .Where(c => c.Id == id)
            .GroupJoin(
                products,
                c => c.CategoryId,
                p => p.CategoryId,
                (c, p) => p.ToList())
            .SingleAsync();
        return queryResult;
    }
}