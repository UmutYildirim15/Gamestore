using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract;

public interface IMongoUnitOfWork
{
    IOrderRepository Orders { get; }

    IOrderDetailRepository OrderDetails { get; }
    ICategoryRepository Categories { get; }

    ISupplierRepository Suppliers { get; }

    IProductRepository Products { get; }
    
    IShipperRepository Shippers { get; }
    
    IGenericRepository<EntityLog> Logs { get; }

    Task<bool> SaveChangesAsync();
}