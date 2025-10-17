using Business.Services.Abstract;
using Business.Services.Resources;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class PlatformService(IPlatformRepository platformRepository, IGameRepository gameRepository, IGamePlatformRepository gamePlatformRepository, ILogger<IPlatformService> logger) : IPlatformService
    {
        public async Task<IEnumerable<PlatformResponse>> GetAllPlatformsAsync()
        {

            var platforms = await platformRepository.GetAllAsync();

            return platforms.Select(p => new PlatformResponse
            {
                Id = p.Id,
                Type = p.Type
            }).ToList();
        }

        public async Task<Guid> CreatePlatformAsync(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                logger.LogWarning("Attempted to create platform with empty or null type.");
                throw new InvalidOperationException(ErrorMessages.NullPlatform);
            }

            var platform = new Platform
            {
                Id = Guid.NewGuid(),
                Type = type
            };

            logger.LogDebug("Creating new platform with ID: {PlatformId} and Type: {PlatformType}", platform.Id, platform.Type);

            await platformRepository.AddAsync(platform);

            logger.LogInformation("Platform with ID: {PlatformId} created successfully.", platform.Id);

            return platform.Id;
        }

        public async Task<PlatformResponse> GetPlatformByIdAsync(Guid id)
        {

            var platform = await platformRepository.GetByIdAsync(id);
            if (platform == null)
            {
                throw new KeyNotFoundException(ErrorMessages.PlatformNotFoundById);
            }


            return new PlatformResponse
            {
                Id = platform.Id,
                Type = platform.Type
            };
        }

        public async Task<bool> UpdatePlatformAsync(Guid id, string type)
        {
            logger.LogDebug("Updating platform with ID: {PlatformId}.", id);

            var existingPlatform = await platformRepository.GetByIdAsync(id);
            if (existingPlatform == null)
            {
                logger.LogError("Platform with ID: {PlatformId} not found for update.", id);
                return false;
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                logger.LogWarning("Attempted to update platform with empty or null type.");
                throw new InvalidOperationException(ErrorMessages.NullPlatform);
            }

            existingPlatform.Type = type;

            logger.LogDebug("Updated platform type to: {PlatformType}.", type);
            await platformRepository.UpdateAsync(existingPlatform);

            logger.LogInformation("Platform with ID: {PlatformId} updated successfully.", id);

            return true;
        }

        public async Task<bool> DeletePlatformAsync(Guid id)
        {
            logger.LogDebug("Deleting platform with ID: {PlatformId}.", id);

            var platform = await platformRepository.GetByIdAsync(id);
            if (platform == null)
            {
                logger.LogError("Platform with ID: {PlatformId} not found for deletion.", id);
                return false;
            }

            await platformRepository.DeleteAsync(id);

            logger.LogInformation("Platform with ID: {PlatformId} deleted successfully.", id);

            return true;
        }

        public async Task<IEnumerable<PlatformResponse?>> GetPlatformsByGameKeyAsync(string key)
        {
            var game = await gameRepository.GetByKeyAsync(key)
                       ?? throw new KeyNotFoundException($"Game not found: {key}");
            var gamePlatforms = await gamePlatformRepository.GetGamePlatformsByGameAsync(game.Id);
            var platforms     = gamePlatforms.Select(gg => gg.Platform);

            return platforms
                .Select(g => g is null
                    ? null
                    : new PlatformResponse { Id = g.Id, Type = g.Type })
                .ToList();
        }

    }
}
