using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Northwind.Business.Services.Abstract;
using Shared.Dto;
using Shared.Models;
using Web.Models.CreateModels;
using Web.Models.RequestModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrdersController(IOrderService orderService, IPaymentService paymentService, IMongoOrderService mongoOrderService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await orderService.GetPaidAndCancelledOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetOrderById([FromRoute] Guid orderId)
        {
            var order = await orderService.GetOrderByIdAsync(orderId);
            return order != null ? Ok(order) : NotFound();
        }

        [HttpGet("{orderId:guid}/details")]
        public async Task<IActionResult> GetOrderDetails([FromRoute] Guid orderId)
        {
            var details = await orderService.GetOrderDetailsAsync(orderId);
            return details.Any() ? Ok(details) : NotFound();
        }

        [HttpGet("cart")]
        public async Task<IActionResult> GetCart()
        {
            var cart = await orderService.GetCartAsync();
            return cart != null && cart.Items.Any() ? Ok(cart) : NotFound();
        }

        [HttpDelete("cart/{key}")]
        public async Task<IActionResult> DeleteGameFromCart([FromRoute] string key)
        {
            var success = await orderService.RemoveGameFromCartAsync(key);
            return success ? NoContent() : NotFound();
        }

        [HttpGet("payment-methods")]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var methods = await paymentService.GetPaymentMethodsAsync();
            return Ok(methods);
        }
        
        [HttpPost("payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] PaymentCreate request)
        {
            var result = await paymentService.ProcessPaymentAsync(
                request.Method, 
                request.Model?.Holder, 
                request.Model?.CardNumber, 
                request.Model?.MonthExpire, 
                request.Model?.YearExpire, 
                request.Model?.Cvv2);
            if (result is BankPaymentResult bankResult && bankResult.PdfBytes != null)
            {
                return File(bankResult.PdfBytes, "application/pdf", bankResult.FileName);
            }
            return Ok(result);
        }
        
        [HttpGet("history")]
        public async Task<IActionResult> GetOrderHistory(DateTime start, DateTime end)
        {
            var filter = new OrderFilter(lowerDate:start, upperDate:end);
            var combinedOrders = await mongoOrderService.GetFilteredOrdersAsync(filter);
            return Ok(combinedOrders);
        }
        
        [HttpPatch("details/{detailId:guid}/quantity")]
        public async Task<IActionResult> UpdateOrderDetailQuantity(
            [FromQuery] Guid customerId, [FromRoute] Guid detailId, [FromBody] OrderDetailQuantityUpdateRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            await orderService.UpdateOrderDetailQuantityAsync(customerId, detailId, request.Count);
            return NoContent();
        }

        // /orders/details/{detailId}?customerId={customerId}
        [HttpDelete("details/{detailId:guid}")]
        public async Task<IActionResult> DeleteOrderDetail([FromQuery] Guid customerId, [FromRoute] Guid detailId)
        {
            await orderService.DeleteOrderDetailAsync(customerId, detailId);
            return NoContent();
        }

        [HttpPost("{id:guid}/ship")]
        public async Task<IActionResult> ShipOrder([FromRoute] Guid id)
        {
            await orderService.ShipOrderAsync(id);
            return NoContent();
        }

        [HttpPost("{id:guid}/details/{key}")]
        public async Task<IActionResult> AddGameToOrderDetail([FromRoute] Guid id, [FromRoute] string key)
        {
            await orderService.AddGameToOrderDetailAsync(id, key);
            return NoContent();
        }


    }
}
