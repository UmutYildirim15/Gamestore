using AutoMapper;
using Business.Services.Dto;
using Northwind.Business.Services.Abstract;
using Northwind.Data.Entities;
using Northwind.Data.Logger;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;

namespace Northwind.Business.Services.Concrete
{
    public class MongoSupplierService(IMongoUnitOfWork unitOfWork, IMapper mapper, IEntityLogger entityLogger) : IMongoSupplierService
    {
        public async Task<PublisherResponse> GetPublisherByIdAsync(string id, string culture)
        {
            var publisher = await unitOfWork.Suppliers.GetOneAsync(s => s.Id == id);
            return mapper.Map<PublisherResponse>(publisher);
        }

        public async Task<IList<PublisherResponse>> GetAllPublishersAsync(string culture)
        {
            var suppliers = await unitOfWork.Suppliers.GetAllAsync();
            return mapper.Map<IList<PublisherResponse>>(suppliers);
        }

        public async Task<IList<GameResponse>> GetGamesByPublisherIdAsync(string id, string culture)
        {
            var products = await unitOfWork.Suppliers.GetProductsBySupplierIdAsync(id);
            return mapper.Map<IList<GameResponse>>(products);
        }
        public async Task CreatePublisherAsync(Supplier newPublisher)
        {
            await unitOfWork.Suppliers.AddAsync(newPublisher);
            await entityLogger.LogCreateAsync(newPublisher);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdatePublisherAsync(string id, Supplier updatedSupplier)
        {
            var existing = await unitOfWork.Suppliers.GetOneAsync(s => s.Id == id);
            if(existing == null)
                throw new KeyNotFoundException($"Publisher with id '{id}' not found.");
            await entityLogger.LogUpdateAsync(existing, updatedSupplier);
            existing.CompanyName = updatedSupplier.CompanyName;
            existing.ContactTitle = updatedSupplier.ContactTitle;
            existing.Address = updatedSupplier.Address;
            existing.City = updatedSupplier.City;
            existing.Region = updatedSupplier.Region;
            existing.PostalCode = updatedSupplier.PostalCode;
            existing.Country = updatedSupplier.Country;
            existing.Phone = updatedSupplier.Phone;
            existing.Fax = updatedSupplier.Fax;
            existing.HomePage = updatedSupplier.HomePage;
            existing.Description = updatedSupplier.Description;
            await unitOfWork.Suppliers.UpdateAsync(existing);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeletePublishersAsync(string? id)
        {
            var publisher = await unitOfWork.Suppliers.GetOneAsync(s => s.Id == id);
            if(publisher == null)
                throw new KeyNotFoundException($"Publisher with id '{id}' not found.");
            await entityLogger.LogDeleteAsync(publisher);
            await unitOfWork.Suppliers.DeleteAsync(id);
            await unitOfWork.SaveChangesAsync();
        }
    }
}
