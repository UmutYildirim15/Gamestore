using Business.Services.Abstract;
using Business.Services.Dto;
using Business.Services.Resources;
using Data.Entities;
using Data.Repositories.Abstract;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class PublisherService(
        IPublisherRepository publisherRepository,
        IGameService gameService,
        ILogger<IPublisherService> logger)
        : IPublisherService
    {
        public async Task<Guid> CreatePublisherAsync(string companyName, string? homePage, string? description)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }

            if (string.IsNullOrWhiteSpace(homePage))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }

            logger.LogInformation("Creating publisher with name: {PublisherName}", companyName);

            var isNameUnique = await publisherRepository.IsNameUniqueAsync(companyName);
            if (!isNameUnique)
            {
                throw new ArgumentException(ErrorMessages.CompanyUnique);
            }

            var publisher = new Publisher
            {
                Id = Guid.NewGuid(),
                CompanyName = companyName,
                HomePage = homePage,
                Description = description
            };

            await publisherRepository.AddAsync(publisher);

            logger.LogInformation("Publisher with ID: {PublisherId} created successfully.", publisher.Id);

            return publisher.Id;
        }

        public async Task<PublisherResponse> GetPublisherByNameAsync(string companyName)
        {
            var publisher = await publisherRepository.GetByNameAsync(companyName);
            if (publisher == null)
                throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);

            return new PublisherResponse
            {
                Id = publisher.Id,
                CompanyName = publisher.CompanyName,
                HomePage = publisher.HomePage,
                Description = publisher.Description
            };
        }

        public async Task<IEnumerable<PublisherResponse>> GetAllPublishersAsync()
        {
            var publishers = await publisherRepository.GetAllAsync();
            return publishers.Select(p => new PublisherResponse
            {
                Id = p.Id,
                CompanyName = p.CompanyName,
                HomePage = p.HomePage,
                Description = p.Description
            }).ToList();
        }

        public async Task<bool> UpdatePublisherAsync(string companyName, string? newCompanyName, string? homePage, string? description)
        {
            if (string.IsNullOrWhiteSpace(newCompanyName))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }

            if (string.IsNullOrWhiteSpace(homePage))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new InvalidOperationException(ErrorMessages.NullOrWhitespacePublisher);
            }
            logger.LogInformation("Updating publisher with Company Name: {CompanyName}", companyName);

            var existingPublisher = await publisherRepository.GetByNameAsync(companyName);
            if (existingPublisher == null)
            {
                throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);
            }
            existingPublisher.CompanyName = newCompanyName;
            existingPublisher.HomePage = homePage;
            existingPublisher.Description = description;

            await publisherRepository.UpdateAsync(existingPublisher);

            logger.LogInformation("Publisher '{CompanyName}' updated successfully.", newCompanyName);
            return true;
        }
        
        public async Task DeletePublisherAsync(Guid id)
        {
            var publisher = await publisherRepository.GetByIdAsync(id);
            if (publisher == null)
                throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);

            var games = await publisherRepository.GetGamesByPublisherIdAsync(id);
            foreach (var game in games)
            {
                await gameService.DeleteGameAsync(game.Id);
            }

            await publisherRepository.DeleteAsync(publisher);
        }
        
        public async Task<IEnumerable<GameResponse>> GetGamesByPublisherNameAsync(string companyName)
        {
            ValidateCompanyName(companyName);
            return await GetGamesByPublisherNameInternalAsync(companyName);
        }
        
        private static void ValidateCompanyName(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
            {
                throw new ArgumentException("companyName parameter cannot be empty");
            }
        }
        
        private async Task<IEnumerable<GameResponse>> GetGamesByPublisherNameInternalAsync(string companyName)
        {
            var publisher = await publisherRepository.GetByNameAsync(companyName);
            if (publisher == null)
            {
                throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);
            }
            var games = await publisherRepository.GetGamesByPublisherIdAsync(publisher.Id);
            return games.Select(g => new GameResponse
            {
                Name = g.Name,
                Key = g.Key,
                Description = g.Description,
                Price = g.Price,
                Discount = g.Discount,
                UnitInStock = g.UnitInStock
            }).ToList();
        }
        
        public async Task<IEnumerable<GameResponse>> GetGamesByPublisherIdAsync(Guid publisherId)
        {
            var games = await publisherRepository.GetGamesByPublisherIdAsync(publisherId);
            return games.Select(g => new GameResponse
            {
                Name = g.Name,
                Key = g.Key,
                Description = g.Description,
                Price = g.Price,
                Discount = g.Discount,
                UnitInStock = g.UnitInStock
            }).ToList();
        }
    }
}
