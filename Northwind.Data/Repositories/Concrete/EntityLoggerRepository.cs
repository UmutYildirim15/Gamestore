using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Repositories.Concrete
{
    public class EntityLoggerRepository(IMongoUnitOfWork unitOfWork) : IEntityLoggerRepository
    {
        private readonly IGenericRepository<EntityLog> _logs = unitOfWork.Logs;

        public async Task AddAsync(EntityLog log)
        {
            await _logs.AddAsync(log);
        }
    }
}