using MongoDB.Driver;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Repositories.Concrete;

public class ShipperRepository(IMongoContext context) : GenericRepository<Shipper>(context), IShipperRepository
{
    private readonly IMongoCollection<Shipper> _shippers = context.GetCollection<Shipper>("Shippers");

    public async Task<List<Shipper>> GetAllAsync()
    {
        return await _shippers.Find(_ => true).ToListAsync();
    }
}