using MongoDB.Driver;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Microsoft.Extensions.Configuration;

namespace Northwind.Data
{
    public class MongoDbContext : IMongoContext
    {
        private readonly IMongoDatabase _database;
        
        public MongoDbContext(IConfiguration configuration)
        {
            var settings = configuration.GetSection("MongoSettings");
            var connectionString = settings["ConnectionString"];
            var databaseName = settings["Database"];

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string? name = null)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = typeof(T).Name;
            }
            return _database.GetCollection<T>(name);
        }

        public void AddCommand(Func<IClientSessionHandle, Task> command)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveChangesAsync()
        {
            return Task.FromResult(true);
        }
        public IMongoCollection<Product> Products => GetCollection<Product>("Products");
        public IMongoCollection<Supplier> Suppliers => GetCollection<Supplier>("Suppliers");
        public IMongoCollection<Order> Orders => GetCollection<Order>("Orders");
        public IMongoCollection<Category> Categories => GetCollection<Category>("Categories");
        public IMongoCollection<OrderDetail> OrderDetails => GetCollection<OrderDetail>("OrderDetails");
        public IMongoCollection<Shipper> Shippers => GetCollection<Shipper>("Shippers");
        public IMongoCollection<EntityLog> Logs => GetCollection<EntityLog>("Logs");
        
    }
}
