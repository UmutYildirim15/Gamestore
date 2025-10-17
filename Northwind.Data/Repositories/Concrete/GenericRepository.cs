using System.Linq.Expressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;
using Shared.Exceptions;

namespace Northwind.Data.Repositories.Concrete
{
    public class GenericRepository<T>(IMongoContext context) : IGenericRepository<T>
        where T : BaseEntity
    {
        protected IMongoContext Context { get; } = context;
        protected IMongoCollection<T> DbSet { get; } = context.GetCollection<T>();

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> predicate)
        {
            var entity = await DbSet.Find(predicate).FirstOrDefaultAsync();
            return entity ?? throw new EntityNotFoundException();
        }

        public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null)
        {
            var filter = predicate ?? (e => true);
            var results = await DbSet.Find(filter).ToListAsync();
            return results;
        }

        public Task AddAsync(T entity)
        {
            Context.AddCommand(session => DbSet.InsertOneAsync(session, entity));
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return await DbSet.AsQueryable().AnyAsync(predicate ?? (e => true));
        }

        public Task UpdateAsync(T entity)
        {
            var filter = Builders<T>.Filter.Eq(e => e.Id, entity.Id);
            Context.AddCommand(session => DbSet.ReplaceOneAsync(session, filter, entity));
            return Task.CompletedTask;
        }

        public Task DeleteAsync(string? id)
        {
            Context.AddCommand(session => DbSet.DeleteOneAsync(session, e => e.Id == id));
            return Task.CompletedTask;
        }
    }
}