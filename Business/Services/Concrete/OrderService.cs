using Business.Services.Abstract;
using Business.Services.Resources;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Shared.Dto;
using Shared.Models;

namespace Business.Services.Concrete
{
    public class OrderService(
        IOrderRepository orderRepository,
        IGameRepository gameRepository,
        ILogger<IOrderService> logger)
        : IOrderService
    {
        private readonly Guid _stubCustomerId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f69afa2");

        public async Task<bool> AddGameToCartAsync(string key)
        {
            logger.LogInformation("Adding game {GameKey} to cart", key);

            var game = await gameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with key '{key}' not found.");
            }

            if (game.UnitInStock <= 0)
            {
                throw new InvalidOperationException($"Game '{game.Name}' is out of stock.");
            }

            var cart = await GetOrCreateCartAsync();

            await AddGameToExistingCartAsync(cart, game);

            return true;
        }

        private async Task<Order> GetOrCreateCartAsync()
        {
            var cart = await orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            if (cart != null)
            {
                return cart;
            }

            cart = new Order
            {
                Id = Guid.NewGuid(),
                CustomerId = _stubCustomerId,
                Status = OrderStatus.Open,
                Date = DateTime.UtcNow,
                OrderGames = new List<OrderGame>()
            };

            await orderRepository.AddAsync(cart);
            return cart;
        }

        private async Task AddGameToExistingCartAsync(Order cart, Game game)
        {
            var existingOrderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);
            if (existingOrderGame != null)
            {
                existingOrderGame.Quantity++;
            }
            else
            {
                cart.OrderGames.Add(new OrderGame
                {
                    OrderId = cart.Id,
                    ProductId = game.Id,
                    Price = game.Price,
                    Quantity = 1,
                    Discount = game.Discount
                });
            }

            await orderRepository.UpdateAsync(cart);
        }

        public async Task<bool> RemoveGameFromCartAsync(string key)
        {
            logger.LogInformation("Removing game {GameKey} from cart", key);

            var cart = await orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            if (cart == null)
            {
                throw new InvalidOperationException("Cart is empty.");
            }

            var game = await gameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with key '{key}' not found.");
            }

            var orderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);
            if (orderGame == null)
            {
                throw new InvalidOperationException("Game is not in the cart.");
            }

            cart.OrderGames.Remove(orderGame);

            if (!cart.OrderGames.Any())
            {
                await orderRepository.DeleteAsync(cart);
            }
            else
            {
                await orderRepository.UpdateAsync(cart);
            }

            return true;
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersAsync()
        {
            var orders = await orderRepository.GetAllAsync();
            return MapOrdersToResponses(orders);
        }

        public async Task<IEnumerable<OrderResponse>> GetPaidAndCancelledOrdersAsync()
        {
            var orders = await orderRepository.GetOrdersByStatusAsync(
                new List<OrderStatus> { OrderStatus.Paid, OrderStatus.Cancelled });
            return MapOrdersToResponses(orders);
        }
        
        public async Task<IEnumerable<OrderResponse>> GetOrdersHistoryAsync(DateTime? startDate, DateTime? endDate)
        {
            if (startDate == null)
            {
                startDate = DateTime.UtcNow.AddDays(-30);
            }
    
            var sqlOrders = await orderRepository.GetAllAsync();
            sqlOrders = sqlOrders.Where(o => o.Date != null && o.Date >= startDate).ToList();
            if (endDate != null)
            {
                sqlOrders = sqlOrders.Where(o => o.Date != null && o.Date <= endDate).ToList();
            }

            var allOrders = MapOrdersToResponses(sqlOrders)
                .OrderByDescending(o => o.Date)
                .ToList();

            return allOrders;
        }

        private static IEnumerable<OrderResponse> MapOrdersToResponses(IEnumerable<Order> orders)
        {
            return orders.Select(o => new OrderResponse
            {
                Id = o.Id,
                CustomerId = o.CustomerId,
                Date = o.Date ?? DateTime.UtcNow
            }).ToList();
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(Guid id)
        {
            var order = await orderRepository.GetByIdAsync(id);
            return order != null
                ? new OrderResponse
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    Date = order.Date ?? DateTime.UtcNow
                }
                : null;
        }

        public async Task<IEnumerable<OrderDetailResponse>> GetOrderDetailsAsync(Guid id)
        {
            var order = await orderRepository.GetByIdAsync(id);
            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID '{id}' not found.");
            }

            return MapOrderGamesToDetailResponses(order.OrderGames);
        }

        public async Task<CartResponse?> GetCartAsync()
        {
            var cart = await orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            if (cart == null) return null;

            return new CartResponse
            {
                Items = MapOrderGamesToDetailResponses(cart.OrderGames).ToList()
            };
        }

        private static IEnumerable<OrderDetailResponse> MapOrderGamesToDetailResponses(
            IEnumerable<OrderGame> orderGames)
        {
            return orderGames.Select(og => new OrderDetailResponse
            {
                ProductId = og.ProductId,
                Price = og.Price,
                Quantity = og.Quantity,
                Discount = og.Discount ?? 0
            });
        }
        
        public async Task UpdateOrderDetailQuantityAsync(Guid customerId, Guid detailId, int count)
        {
            var cart = await orderRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
                throw new InvalidOperationException(ErrorMessages.CartNotFound);

            var orderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == detailId);
            if (orderGame == null)
                throw new KeyNotFoundException(ErrorMessages.OrderDetailNotFound);

            orderGame.Quantity = count;
            await orderRepository.UpdateAsync(cart);
            logger.LogInformation("OrderDetail with ID {orderId} is updated", detailId);
        }
        
        public async Task DeleteOrderDetailAsync(Guid customerId, Guid detailId)
        {
            var cart = await orderRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
                throw new InvalidOperationException(ErrorMessages.CartNotFound);

            var orderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == detailId);
            if (orderGame == null)
                throw new KeyNotFoundException(ErrorMessages.OrderDetailNotFound);

            cart.OrderGames.Remove(orderGame);
            await orderRepository.UpdateAsync(cart);
            logger.LogInformation("OrderDetail with ID {orderId} is deleted", detailId);
        }

        public async Task ShipOrderAsync(Guid orderId)
        {
            var order = await orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException(ErrorMessages.OrderNotFound);

            if (order.Status != OrderStatus.Paid)
                throw new InvalidOperationException("Only paid orders can be shipped.");

            order.Status = OrderStatus.Shipped;
            await orderRepository.UpdateAsync(order);
            logger.LogInformation("Order with ID {orderId} is shipped", orderId);
        }

        public async Task AddGameToOrderDetailAsync(Guid orderId, string key)
        {
            logger.LogInformation("Adding game with key {GameKey} to order {OrderId}", key, orderId);

            var game = await gameRepository.GetByKeyAsync(key);
            if (game == null)
                throw new KeyNotFoundException($"Game with key '{key}' not found.");

            var order = await orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new KeyNotFoundException($"Order with id '{orderId}' not found.");

            var orderGame = order.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);
            if (orderGame != null)
            {
                orderGame.Quantity++;
            }
            else
            {
                order.OrderGames.Add(new OrderGame
                {
                    OrderId = order.Id,
                    ProductId = game.Id,
                    Price = game.Price,
                    Quantity = 1,
                    Discount = game.Discount
                });
            }
            await orderRepository.UpdateAsync(order);
        }
    }
}