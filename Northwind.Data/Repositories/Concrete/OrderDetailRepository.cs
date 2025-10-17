using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Repositories.Concrete;

public class OrderDetailRepository(IMongoContext context)
    : GenericRepository<OrderDetail>(context), IOrderDetailRepository
{
    public async Task<IList<OrderDetail>> GetAllByOrderObjectIdAsync(string id)
    {
        var orders = Context.GetCollection<Order>().AsQueryable();
        var details = DbSet.AsQueryable();

        var query = await orders.Where(o => o.Id == id)
            .GroupJoin(
                details,
                o => o.OrderId,
                d => d.OrderId,
                (o, d) => d.ToList())
            .SingleAsync();
        return query;
    }
}