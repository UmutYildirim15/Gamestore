using Northwind.Data.Entities;

namespace Northwind.Data.Repositories.Abstract
{
    public interface IEntityLoggerRepository
    {
        Task AddAsync(EntityLog log);
    }
}

