using Business.Services.Abstract;
using Data.Repositories.Abstract;
using Business.Services.Resources;
using Data.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class GenreService(IGenreRepository genreRepository, IGameRepository gameRepository, IGameGenreRepository gameGenreRepository, ILogger<IGenreService> logger) : IGenreService
    {
        public async Task<IEnumerable<GenreResponse>> GetAllGenresAsync()
        {
            var genres = await genreRepository.GetAllAsync();
            return genres.Select(g => new GenreResponse
            {
                Id = g.Id,
                Name = g.Name
            }).ToList();
        }
        
        public async Task<Guid> CreateGenreAsync(string name, string? parentGenreName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Attempted to create a genre with an empty name.");
                throw new InvalidOperationException("Genre name cannot be empty.");
            }

            logger.LogInformation("Creating a new genre with name: {GenreName}.", name);

            Genre? parentGenre = null;

            if (!string.IsNullOrWhiteSpace(parentGenreName))
            {
                parentGenre = await genreRepository.GetByNameAsync(parentGenreName);
                if (parentGenre == null)
                {
                    logger.LogWarning("Parent genre with name '{ParentGenreName}' not found.", parentGenreName);
                    throw new InvalidOperationException($"Parent genre '{parentGenreName}' not found.");
                }
            }

            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = name,
                ParentGenreId = parentGenre?.Id 
            };

            logger.LogDebug("Generated new Genre ID: {GenreId}.", genre.Id);

            await genreRepository.AddAsync(genre);
            logger.LogInformation("Genre with ID: {GenreId} created successfully.", genre.Id);

            return genre.Id;
        }

        public async Task<GenreResponse?> GetGenreByIdAsync(Guid id)
        {
            var genre = await genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                return null;
            }
            return new GenreResponse
            {
                Id = genre.Id,
                Name = genre.Name,
                ParentGenreId = genre.ParentGenreId
            };
        }
        
        public async Task<bool> UpdateGenreAsync(Guid id, string name)
        {
            logger.LogDebug("Updating genre with ID: {GenreId}.", id);
            var existingGenre = await genreRepository.GetByIdAsync(id);
            if (existingGenre == null)
            {
                logger.LogWarning("No genre found with ID: {GenreId} for update.", id);
                return false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Attempted to update genre with an empty name.");
                throw new InvalidOperationException(ErrorMessages.NullGenre);
            }

            existingGenre.Name = name;

            logger.LogDebug("Updated genre name to: {GenreName}.", name);

            await genreRepository.UpdateAsync(existingGenre);
            logger.LogInformation("Genre with ID: {GenreId} updated successfully.", id);

            return true;
        }
        
        public async Task<bool> DeleteGenreAsync(Guid id)
        {
            logger.LogDebug("Deleting genre with ID: {GenreId}.", id);

            var genre = await genreRepository.GetByIdAsync(id);
            if (genre == null)
            {
                logger.LogWarning("No genre found with ID: {GenreId} for deletion.", id);
                return false;
            }

            await genreRepository.DeleteAsync(id);
            logger.LogInformation("Genre with ID: {GenreId} deleted successfully.", id);

            return true;
        }
        
        public async Task<IEnumerable<GenreResponse?>> GetGenresByGameKeyAsync(string key)
        {
            var game = await gameRepository.GetByKeyAsync(key)
                       ?? throw new KeyNotFoundException($"Game not found: {key}");
            var gameGenres = await gameGenreRepository.GetGameGenresByGameAsync(game.Id);
            var genres = gameGenres.Select(gg => gg.Genre);

            return genres.Select(g =>
            {
                if (g != null)
                    return new GenreResponse
                    {
                        Id = g.Id,
                        Name = g.Name,
                        ParentGenreId = g.ParentGenreId
                    };
                return null;
            }).ToList();
        }
        
        public async Task<IEnumerable<GenreResponse>> GetGenresByParentIdAsync(Guid parentId)
        {
            var genres = await genreRepository.GetGenresByParentIdAsync(parentId);
            return genres.Select(g => new GenreResponse
            {
                Id = g.Id,
                Name = g.Name,
                ParentGenreId = g.ParentGenreId
            }).ToList();
        }
    }
}