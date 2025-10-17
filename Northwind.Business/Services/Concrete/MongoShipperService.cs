using Northwind.Business.Services.Abstract;
using Northwind.Data.Repositories.Abstract;
using Northwind.Data.Entities;
using Northwind.Data.Logger;

namespace Northwind.Business.Services.Concrete
{
    public class MongoShipperService(IMongoUnitOfWork unitOfWork, IEntityLogger entityLogger) : IMongoShipperService
    {
        public async Task<List<Shipper>> GetShippersAsync()
        {
            return await unitOfWork.Shippers.GetAllAsync();
        }

        public async Task CreateShipperAsync(Shipper newShipper)
        {
            await unitOfWork.Shippers.AddAsync(newShipper);
            await entityLogger.LogCreateAsync(newShipper);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateShipperAsync(string id, Shipper updatedShipper)
        {
            var shipper = await unitOfWork.Shippers.GetOneAsync(s => s.Id == id);
            if(shipper == null)
                throw new KeyNotFoundException($"Shipper with id '{id}' not found.");
            await entityLogger.LogUpdateAsync(shipper, updatedShipper);
            shipper.CompanyName = updatedShipper.CompanyName;
            shipper.Phone = updatedShipper.Phone;
            await unitOfWork.Shippers.UpdateAsync(shipper);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteShipperAsync(string id)
        {
            var shipper = await unitOfWork.Shippers.GetOneAsync(s => s.Id == id);
            if(shipper == null)
                throw new KeyNotFoundException($"Shipper with id '{id}' not found.");
            await entityLogger.LogDeleteAsync(shipper);
            await unitOfWork.Shippers.DeleteAsync(id);
            await unitOfWork.SaveChangesAsync();
        }
    }
}