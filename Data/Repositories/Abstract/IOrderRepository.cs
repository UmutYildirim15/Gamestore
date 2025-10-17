using Data.Entities;

namespace Data.Repositories.Abstract
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync();
        Task<Order?> GetByIdAsync(Guid id);
        Task<Order?> GetCartByCustomerIdAsync(Guid customerId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
        Task DeleteAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(IEnumerable<OrderStatus> statuses);
    }
}