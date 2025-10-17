using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Shared.Models;

namespace Northwind.Data.Repositories.Concrete;

public class OrderRepository(IMongoContext context) : GenericRepository<Order>(context), IOrderRepository
{
    public async Task<IList<Order>> GetFilteredOrdersAsync(OrderFilter filter)
    {
        var query = DbSet.AsQueryable();
        if (filter.CustomerId is not null)
        {
            query = query.Where(o => o.CustomerId == filter.CustomerId);
        }

        return await query.Where(o => o.OrderDate >= filter.LowerDate && o.OrderDate <= filter.UpperDate).ToListAsync();
    }
}