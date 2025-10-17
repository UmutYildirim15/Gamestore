using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract;

public interface IOrderDetailRepository : IGenericRepository<OrderDetail>
{
    Task<IList<OrderDetail>> GetAllByOrderObjectIdAsync(string id);
}