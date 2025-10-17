using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data;

public class MongoUnitOfWork(
    IMongoContext context,
    OrderDataAccess orderDataAccess,
    IProductRepository productsRepository,
    ICategoryRepository categoriesRepository,
    ISupplierRepository suppliersRepository,
    IShipperRepository shipperRepository,
    IGenericRepository<EntityLog> entityLogRepository)
    : IMongoUnitOfWork
{
    public IOrderRepository Orders => orderDataAccess.Orders;
    public IOrderDetailRepository OrderDetails => orderDataAccess.OrderDetails;
    public IProductRepository Products { get; } = productsRepository;
    public ICategoryRepository Categories { get; } = categoriesRepository;
    public ISupplierRepository Suppliers { get; } = suppliersRepository;
    public IShipperRepository Shippers { get; } = shipperRepository;
    public IGenericRepository<EntityLog> Logs { get; } = entityLogRepository;

    public Task<bool> SaveChangesAsync() => context.SaveChangesAsync();
}
public class OrderDataAccess(IOrderRepository orders, IOrderDetailRepository orderDetails)
{
    public IOrderRepository Orders { get; } = orders;
    public IOrderDetailRepository OrderDetails { get; } = orderDetails;
}