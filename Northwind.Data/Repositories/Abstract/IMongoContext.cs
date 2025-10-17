using MongoDB.Driver;

namespace Northwind.Data.Repositories.Abstract;

public interface IMongoContext
{
    IMongoCollection<T> GetCollection<T>(string? name = null);

    void AddCommand(Func<IClientSessionHandle, Task> command);

    Task<bool> SaveChangesAsync();
}