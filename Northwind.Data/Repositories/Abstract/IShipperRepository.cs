using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract;

public interface IShipperRepository : IGenericRepository<Shipper>
{
    Task<List<Shipper>> GetAllAsync();
}