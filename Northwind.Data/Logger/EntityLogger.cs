using MongoDB.Bson;
using Newtonsoft.Json;
using Northwind.Data.Entities;
using Northwind.Data.Repositories.Abstract;

namespace Northwind.Data.Logger
{
    public class EntityLogger(IMongoUnitOfWork unitOfWork) : IEntityLogger
    {
        private const string CreateAction = "CREATE";
        private const string UpdateAction = "UPDATE";
        private const string DeleteAction = "DELETE";

        public async Task LogCreateAsync(object entity)
        {
            await unitOfWork.Logs.AddAsync(GetEntityLog(CreateAction, entity));
            await unitOfWork.SaveChangesAsync();
        }

        public async Task LogUpdateAsync(object oldEntity, object updEntity)
        {
            await unitOfWork.Logs.AddAsync(GetEntityLog(UpdateAction, oldEntity, updEntity));
            await unitOfWork.SaveChangesAsync();
        }

        public async Task LogDeleteAsync(object entity)
        {
            await unitOfWork.Logs.AddAsync(GetEntityLog(DeleteAction, entity));
            await unitOfWork.SaveChangesAsync();
        }

        private static EntityLog GetEntityLog(string action, object entity)
        {
            return new EntityLog
            {
                Action = action,
                Date = DateTime.UtcNow,
                EntityType = entity.GetType().ToString(),
                Entity = GetBsonDocument(entity)
            };
        }

        private static EntityLog GetEntityLog(string action, object oldEntity, object updEntity)
        {
            var log = GetEntityLog(action, oldEntity);
            log.UpdEntity = GetBsonDocument(updEntity);
            return log;
        }

        private static BsonDocument GetBsonDocument(object entity)
        {
            string json = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            return BsonDocument.Parse(json);
        }
    }
}
