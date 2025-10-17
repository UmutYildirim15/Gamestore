using Business.Services.Dto;
using Microsoft.Extensions.Logging;

namespace Business.Services.Resources
{
    public static class GameValidationHelper
    {
        public static void ValidateCreateDto(GameCreateDto dto, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                logger.LogWarning("Attempted to create game with no name.");
                throw new InvalidOperationException(ErrorMessages.NoName);
            }

            if (dto.Price <= 0)
            {
                logger.LogWarning("Invalid price: {Price}", dto.Price);
                throw new InvalidOperationException("Price must be greater than zero.");
            }

            if (dto.UnitInStock < 0)
            {
                logger.LogWarning("Invalid unit in stock: {UnitInStock}", dto.UnitInStock);
                throw new InvalidOperationException("UnitInStock cannot be negative.");
            }

            if (dto.Discount < 0 || dto.Discount > 100)
            {
                logger.LogWarning("Invalid discount: {Discount}", dto.Discount);
                throw new InvalidOperationException("Discount must be between 0 and 100.");
            }

            if (dto.GenreIds == null || !dto.GenreIds.Any())
            {
                logger.LogWarning("Attempted to create game without genres.");
                throw new InvalidOperationException(ErrorMessages.GenreNotFound);
            }

            if (dto.PlatformIds == null || !dto.PlatformIds.Any())
            {
                logger.LogWarning("Attempted to create game without platforms.");
                throw new InvalidOperationException(ErrorMessages.PlatformNotFound);
            }
        }

        public static void ValidateUpdateDto(GameCreateDto dto, ILogger logger)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                logger.LogWarning("Attempted to update game with no name.");
                throw new InvalidOperationException(ErrorMessages.NoName);
            }

            if (string.IsNullOrWhiteSpace(dto.Key))
            {
                logger.LogWarning("Attempted to update game with no key.");
                throw new InvalidOperationException(ErrorMessages.NoKey);
            }

            ValidateCreateDto(dto, logger);
        }

        public static void EnsureEntityNotNull<T>(T entity, string logMessage, string exceptionMessage, ILogger logger)
            where T : class
        {
            if (entity == null)
            {
                logger.LogError(logMessage);
                throw new KeyNotFoundException(exceptionMessage);
            }
        }

        public static void EnsureCollectionExists<T>(IEnumerable<T> loaded, int expectedCount, string exceptionMessage)
        {
            if (loaded.Count() != expectedCount)
                throw new KeyNotFoundException(exceptionMessage);
        }

        public static void EnsureStockAvailable(int stock, ILogger logger)
        {
            if (stock <= 0)
            {
                logger.LogWarning("Item is out of stock.");
                throw new InvalidOperationException(ErrorMessages.GameOutOfStock);
            }
        }
    }
}