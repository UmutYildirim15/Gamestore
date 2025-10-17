using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IOrderService
    {
        Task<bool> AddGameToCartAsync(string key);
        Task<bool> RemoveGameFromCartAsync(string key);
        Task<IEnumerable<OrderResponse>> GetOrdersAsync();
        Task<OrderResponse?> GetOrderByIdAsync(Guid id);
        Task<IEnumerable<OrderDetailResponse>> GetOrderDetailsAsync(Guid id);
        Task<CartResponse?> GetCartAsync();

        Task<IEnumerable<OrderResponse>> GetPaidAndCancelledOrdersAsync();
        Task<IEnumerable<OrderResponse>> GetOrdersHistoryAsync(DateTime? startDate, DateTime? endDate);
        Task UpdateOrderDetailQuantityAsync(Guid customerId, Guid detailId, int count);
        Task DeleteOrderDetailAsync(Guid customerId, Guid detailId);
        Task ShipOrderAsync(Guid orderId);
        Task AddGameToOrderDetailAsync(Guid orderId, string key);

    }
}