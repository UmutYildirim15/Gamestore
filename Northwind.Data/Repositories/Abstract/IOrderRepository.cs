using Northwind.Data.Entities;
using Shared.Models;

namespace Northwind.Data.Repositories.Abstract;

public interface IOrderRepository : IGenericRepository<Order>
{
    Task<IList<Order>> GetFilteredOrdersAsync(OrderFilter filter);
}