using Shared.Dto;
using Shared.Models;

namespace Northwind.Business.Services.Abstract;

public interface IMongoOrderService
{
    Task<IList<OrderResponse>> GetFilteredOrdersAsync(OrderFilter filter);
    Task<OrderResponse> GetOrderByIdAsync(string orderId);
    Task<IList<OrderDetailResponse>> GetOrderDetailsAsync(string orderId);
}