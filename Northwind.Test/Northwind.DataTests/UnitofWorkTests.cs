using Moq;
using Northwind.Data;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Xunit;

namespace Northwind.Test.Northwind.DataTests;

public class MongoUnitOfWorkTests
{
    private readonly Mock<IMongoContext> _contextMock = new();
    private readonly Mock<IOrderRepository> _orderRepoMock = new();
    private readonly Mock<IOrderDetailRepository> _orderDetailsRepoMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly Mock<ICategoryRepository> _categoryRepoMock = new();
    private readonly Mock<ISupplierRepository> _supplierRepoMock = new();
    private readonly Mock<IShipperRepository> _shipperRepoMock = new();
    private readonly Mock<IGenericRepository<EntityLog>> _entityLogsRepoMock = new();
    private readonly MongoUnitOfWork _mongoUnitOfWork;

    public MongoUnitOfWorkTests()
    {
        var orderDataAccess = new OrderDataAccess(
            _orderRepoMock.Object,
            _orderDetailsRepoMock.Object
        );

        _mongoUnitOfWork = new MongoUnitOfWork(
            _contextMock.Object,
            orderDataAccess,
            _productRepoMock.Object,
            _categoryRepoMock.Object,
            _supplierRepoMock.Object,
            _shipperRepoMock.Object,
            _entityLogsRepoMock.Object);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenCalled_CallsContext()
    {
        // Arrange
        _contextMock.Setup(c => c.SaveChangesAsync()).ReturnsAsync(true).Verifiable();

        // Act
        await _mongoUnitOfWork.SaveChangesAsync();

        // Assert
        _contextMock.Verify(c => c.SaveChangesAsync(), Times.Once);
    }
}