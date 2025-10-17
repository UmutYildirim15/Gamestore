using Northwind.Data.Entities;

namespace Northwind.Business.Services.Abstract;

public interface IMongoShipperService
{
    Task<List<Shipper>> GetShippersAsync();
}