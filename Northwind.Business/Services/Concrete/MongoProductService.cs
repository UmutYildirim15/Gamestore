using System.Text;
using AutoMapper;
using Business.Services.Dto;
using Data.Entities;
using Northwind.Business.Services.Abstract;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Shared.Dto;
using Shared.Models;
using Northwind.Data.Logger;

namespace Northwind.Business.Services.Concrete
{
    public class MongoProductService(IMongoUnitOfWork unitOfWork, IMapper mapper, IEntityLogger entityLogger) : IMongoProductService
    {
        public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
        {
            var products = await unitOfWork.Products.GetAllAsync();
            return mapper.Map<IEnumerable<GameResponse>>(products);
        }
        
        public async Task<GameResponse> GetGameByKeyAsync(string key, string culture)
        {
            var product = await unitOfWork.Products.GetOneAsync(p => p.Key == key);
            if(product == null)
                throw new KeyNotFoundException($"Product with key '{key}' not found.");
            return mapper.Map<GameResponse>(product);
        }

        public async Task<GameResponse> GetGameByIdAsync(string id, string culture)
        {
            long productId = long.Parse(id);
            var product = await unitOfWork.Products.GetOneAsync(p => p.ProductId == productId);
            if(product == null)
                throw new KeyNotFoundException($"Product with id '{id}' not found.");
            var response = mapper.Map<GameResponse>(product);
            response.Description = product.ProductName;
            return response;
        }

        public async Task<IList<GenreResponse>> GetGenresByGameKeyAsync(string key)
        {
            var categories = await unitOfWork.Products.GetCategoriesByProductKeyAsync(key);
            return mapper.Map<IList<GenreResponse>>(categories);
        }

        public Task<IList<PlatformResponse>> GetPlatformsByGameKeyAsync(string key)
        {
            return Task.FromResult((IList<PlatformResponse>)new List<PlatformResponse>());
        }

        public async Task<PublisherResponse> GetPublisherByGameKeyAsync(string key)
        {
            var supplier = await unitOfWork.Products.GetSupplierByProductKeyAsync(key);
            if(supplier == null)
                throw new KeyNotFoundException("Supplier not found for the given game alias.");
            return mapper.Map<PublisherResponse>(supplier);
        }

        public async Task<EntityFilter<GameResponse>> GetFilteredGamesAsync(GameFilter filter)
        {
            IList<GameResponse> records = new List<GameResponse>();
            int totalNoLimit = 0;
            if (filter.Platforms != null && filter.Platforms.Count == 0 && filter.DatePublishing is null)
            {
                var filteringResult = await unitOfWork.Products.GetFilteredProductsAsync(filter);
                records = mapper.Map<IList<GameResponse>>(filteringResult.Records);
                totalNoLimit = filteringResult.TotalNoLimit;
            }
            return new EntityFilter<GameResponse>(records, totalNoLimit);
        }

        public async Task CreateGameAsync(Product newProduct)
        {
            await unitOfWork.Products.AddAsync(newProduct);
            await entityLogger.LogCreateAsync(newProduct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task UpdateGameAsync(string key, Product updatedProduct)
        {
            var product = await unitOfWork.Products.GetOneAsync(p => p.Key == key);
            if(product == null)
                throw new KeyNotFoundException($"Product with key '{key}' not found.");
            await unitOfWork.Products.UpdateAsync(updatedProduct);
            await entityLogger.LogUpdateAsync(key, updatedProduct);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteGameAsync(string key)
        {
            var product = await unitOfWork.Products.GetOneAsync(p => p.Key == key);
            if (product == null)
                throw new KeyNotFoundException($"Product with key '{key}' not found.");
            if (product.Deleted)
                await unitOfWork.Products.DeleteAsync(product.Id);
            else
            {
                product.Deleted = true;
                await unitOfWork.Products.UpdateAsync(product);
            }
            await entityLogger.LogDeleteAsync(key);
            await unitOfWork.SaveChangesAsync();
        }

        public async Task<Tuple<byte[], string>> DownloadAsync(string gameKey)
        {
            var game = await unitOfWork.Products.GetOneAsync(g => g.Key == gameKey);
            if(game == null)
                throw new KeyNotFoundException($"Game with key '{gameKey}' not found.");
            string content = $"Game: {game.ProductName}\n\nDescription:";
            byte[] bytes = Encoding.UTF8.GetBytes(content);
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
            string fileName = $"{game.ProductName}_{timestamp}.txt";
            return new Tuple<byte[], string>(bytes, fileName);
        }

        public async Task SyncStockAfterPaymentAsync(IEnumerable<OrderGame> orderGames)
        {
            foreach (var orderGame in orderGames)
            {
                var product = await unitOfWork.Products.GetOneAsync(p => p.ProductId.ToString() == orderGame.ProductId.ToString());
                product.UnitsInStock = (short)Math.Max(product.UnitsInStock - orderGame.Quantity, 0);
                await unitOfWork.Products.UpdateAsync(product);
            }
            await unitOfWork.SaveChangesAsync();
        }
    }
}
