using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Concrete
{
    public class OrderRepository(ApplicationDbContext context) : IOrderRepository
    {
        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await context.Orders
                .Include(o => o.OrderGames)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(Guid id)
        {
            return await context.Orders
                .Include(o => o.OrderGames)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order?> GetCartByCustomerIdAsync(Guid customerId)
        {
            return await context.Orders
                .Include(o => o.OrderGames)
                .FirstOrDefaultAsync(o => o.CustomerId == customerId && o.Status == OrderStatus.Open);
        }

        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Order order)
        {
            context.Orders.Remove(order);
            await context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(IEnumerable<OrderStatus> statuses)
        {
            return await context.Orders
                .Where(o => statuses.Contains(o.Status))
                .ToListAsync();
        }
    }
}