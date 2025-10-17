using AutoMapper;
using Northwind.Business.Services.Abstract;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Shared.Models;
using Northwind.Data.Logger;

namespace Northwind.Business.Services.Concrete
{
    public class MongoOrderService(IMongoUnitOfWork unitOfWork, IMapper mapper, IEntityLogger entityLogger) : IMongoOrderService
    {
        public async Task<IList<OrderResponse>> GetFilteredOrdersAsync(OrderFilter filter)
        {
            var orders = await unitOfWork.Orders.GetFilteredOrdersAsync(filter);
            return mapper.Map<IList<OrderResponse>>(orders);
        }

        public async Task<OrderResponse> GetOrderByIdAsync(string orderId)
        {
            var order = await unitOfWork.Orders.GetOneAsync(o => o.Id == orderId);
            return mapper.Map<OrderResponse>(order);
        }

        public async Task<IList<OrderDetailResponse>> GetOrderDetailsAsync(string orderId)
        {
            var details = await unitOfWork.OrderDetails.GetAllByOrderObjectIdAsync(orderId);
            return mapper.Map<IList<OrderDetailResponse>>(details);
        }

        public async Task CreateOrderAsync(Data.Entities.Order order)
        {
            await unitOfWork.Orders.AddAsync(order);
            await entityLogger.LogCreateAsync(order);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(string orderId, Data.Entities.Order updatedOrder)
        {
            var existing = await unitOfWork.Orders.GetOneAsync(o => o.Id == orderId);
            await entityLogger.LogUpdateAsync(existing, updatedOrder);
            existing.OrderDate = updatedOrder.OrderDate;
            existing.CustomerId = updatedOrder.CustomerId;
            existing.OrderDetails = updatedOrder.OrderDetails;
            await unitOfWork.Orders.UpdateAsync(existing);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(string orderId)
        {
            var order = await unitOfWork.Orders.GetOneAsync(o => o.Id == orderId);
            await entityLogger.LogDeleteAsync(order);
            await unitOfWork.Orders.DeleteAsync(order.Id);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
