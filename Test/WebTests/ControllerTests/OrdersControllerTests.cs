using Business.Services.Abstract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Northwind.Business.Services.Abstract;
using Shared.Dto;
using Test.Helpers;
using Web.Controllers;

namespace Test.WebTests.ControllerTests
{
    public class OrdersControllerTests
    {
        private readonly Mock<IOrderService> _mockOrderService;
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<IMongoOrderService> _mockMongoOrderService;
        private readonly OrdersController _controller;

        public OrdersControllerTests()
        {
            _mockPaymentService = new Mock<IPaymentService>();
            _mockOrderService = new Mock<IOrderService>();
            _mockMongoOrderService = new Mock<IMongoOrderService>();
            _controller = new OrdersController(_mockOrderService.Object, _mockPaymentService.Object, _mockMongoOrderService.Object);
        }

        [Fact]
        public async Task GetOrders_ReturnsOkResult_WithOrders()
        {
            var orders = WebTestHelper.CreateOrderResponseList(3);
            _mockOrderService.Setup(service => service.GetPaidAndCancelledOrdersAsync()).ReturnsAsync(orders);

            var result = await _controller.GetOrders();
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orders, okResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ReturnsOkResult_WhenOrderExists()
        {
            var order = WebTestHelper.CreateSingleOrderResponse();
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

            var result = await _controller.GetOrderById(order.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(order, okResult.Value);
        }

        [Fact]
        public async Task GetOrderById_ReturnsNotFound_WhenOrderDoesNotExist()
        {
            var orderId = Guid.NewGuid();
            _mockOrderService.Setup(service => service.GetOrderByIdAsync(orderId)).ReturnsAsync((OrderResponse?)null);

            var result = await _controller.GetOrderById(orderId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsOk_WhenDetailsExist()
        {
            var orderDetails = WebTestHelper.CreateOrderDetailList(2);
            var orderId = Guid.NewGuid();
            _mockOrderService.Setup(service => service.GetOrderDetailsAsync(orderId)).ReturnsAsync(orderDetails);

            var result = await _controller.GetOrderDetails(orderId);
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(orderDetails, okResult.Value);
        }

        [Fact]
        public async Task GetOrderDetails_ReturnsNotFound_WhenNoDetailsExist()
        {
            var orderId = Guid.NewGuid();
            _mockOrderService.Setup(service => service.GetOrderDetailsAsync(orderId)).ReturnsAsync(new List<OrderDetailResponse>());

            var result = await _controller.GetOrderDetails(orderId);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task DeleteGameFromCart_ReturnsNoContent_WhenSuccessful()
        {
            _mockOrderService.Setup(service => service.RemoveGameFromCartAsync(It.IsAny<string>())).ReturnsAsync(true);

            var result = await _controller.DeleteGameFromCart("testKey");
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteGameFromCart_ReturnsNotFound_WhenFailed()
        {
            _mockOrderService.Setup(service => service.RemoveGameFromCartAsync(It.IsAny<string>())).ReturnsAsync(false);

            var result = await _controller.DeleteGameFromCart("testKey");
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ProcessPayment_ReturnsFile_WhenPaymentSuccessful()
        {
            var request = WebTestHelper.CreatePaymentRequest();
            var paymentResult = WebTestHelper.CreateBankPaymentResult();

            _mockPaymentService.Setup(service => service.ProcessPaymentAsync(
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), 
                    It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(paymentResult); 

            var result = await _controller.ProcessPayment(request);
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/pdf", fileResult.ContentType);
        }
    }
}

