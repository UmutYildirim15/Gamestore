using System.Text;
using Business.Facades;
using Business.Filtering;
using Business.Filtering.Operations;
using Business.Filtering.Steps;
using Business.Services.Abstract;
using Business.Services.Dto;
using Business.Services.Resources;
using Data.Entities;
using Microsoft.Extensions.Logging;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class GameService(
        GameRepositoryFacade repositoryFacade, ILogger<IGameService> logger) : IGameService
    {
        public async Task<IEnumerable<GameResponse>> GetAllGamesAsync()
        {
            var games = await repositoryFacade.GameRepository.GetAllAsync();
            return games.Select(g => new GameResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description ?? ErrorMessages.NoDescription,
                Key = g.Key,
                Price = g.Price,
                UnitInStock = g.UnitInStock,
                Discount = g.Discount
            }).ToList();
        }

        public async Task<Guid> CreateGameAsync(GameCreateDto gameDto)
        {
            GameValidationHelper.ValidateCreateDto(gameDto, logger);

            if (string.IsNullOrWhiteSpace(gameDto.Key))
                gameDto.Key = await GenerateKeyFromName(gameDto.Name);

            logger.LogInformation("Creating game {Name} ({Key})", gameDto.Name, gameDto.Key);

            var genreIds = gameDto.GenreIds!.ToList();
            var platformIds = gameDto.PlatformIds!.ToList();

            var genres = await repositoryFacade.GenreRepository.GetByIdsAsync(genreIds);
            var platforms = await repositoryFacade.PlatformRepository.GetByIdsAsync(platformIds);
            GameValidationHelper.EnsureCollectionExists(genres, genreIds.Count, ErrorMessages.GenreNotFound);
            GameValidationHelper.EnsureCollectionExists(platforms, platformIds.Count, ErrorMessages.PlatformNotFound);

            var game = new Game
            {
                Id = Guid.NewGuid(),
                Name = gameDto.Name,
                Key = gameDto.Key,
                Description = gameDto.Description,
                Price = gameDto.Price,
                UnitInStock = gameDto.UnitInStock,
                Discount = gameDto.Discount,
                PublisherId = gameDto.PublisherId,
                Genres = genreIds.Select(id => new GameGenre { GameId = Guid.NewGuid(), GenreId = id }).ToList(),
                Platforms = platformIds.Select(id => new GamePlatform { GameId = Guid.NewGuid(), PlatformId = id }).ToList()
            };

            await repositoryFacade.GameRepository.AddAsync(game);
            logger.LogInformation("Game created: {Id}", game.Id);
            return game.Id;
        }
        
        public async Task<GameResponse?> GetGameByIdAsync(Guid id)
        {
            var game = await repositoryFacade.GameRepository.GetByIdAsync(id);
            if (game == null)
            {
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundById, id));
            }

            return new GameResponse
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description ?? ErrorMessages.NoDescription,
                Key = game.Key,
                Price = game.Price,
                UnitInStock = game.UnitInStock,
                Discount = game.Discount
            };
        }

        public async Task<bool> UpdateGameAsync(Guid id, GameCreateDto gameDto)
        {
            logger.LogInformation("Updating game with ID: {GameId}", id);

            var existing = await repositoryFacade.GameRepository.GetByIdAsync(id)
                          ?? throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundById, id));

            GameValidationHelper.ValidateUpdateDto(gameDto, logger);

            var genreIds = gameDto.GenreIds!.ToList();
            var platformIds = gameDto.PlatformIds!.ToList();
            _ = await repositoryFacade.PublisherRepository.GetByIdAsync(gameDto.PublisherId)
                ?? throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);

            var genres = await repositoryFacade.GenreRepository.GetByIdsAsync(genreIds);
            var platforms = await repositoryFacade.PlatformRepository.GetByIdsAsync(platformIds);

            if (genres.Count() != genreIds.Count)
                throw new KeyNotFoundException(ErrorMessages.GenreNotFound);
            if (platforms.Count() != platformIds.Count)
                throw new KeyNotFoundException(ErrorMessages.PlatformNotFound);

            existing.Name = gameDto.Name;
            if (gameDto.Key != null) existing.Key = gameDto.Key;
            existing.Description = gameDto.Description;
            existing.Price = gameDto.Price;
            existing.UnitInStock = gameDto.UnitInStock;
            existing.Discount = gameDto.Discount;
            existing.PublisherId = gameDto.PublisherId;
            existing.Genres = genreIds
                .Where(g => g != Guid.Empty)
                .Select(g => new GameGenre { GameId = existing.Id, GenreId = g }).ToList();
            existing.Platforms = platformIds
                .Where(p => p != Guid.Empty)
                .Select(p => new GamePlatform { GameId = existing.Id, PlatformId = p }).ToList();

            await repositoryFacade.GameRepository.UpdateAsync(existing);
            logger.LogInformation("Game with ID: {GameId} updated successfully.", id);

            return true;
        }
        public async Task<bool> DeleteGameAsync(Guid id)
        {
            logger.LogInformation("Deleting game with ID: {GameId}", id);

            var game = await repositoryFacade.GameRepository.GetByIdAsync(id);
            if (game == null)
            {
                logger.LogError("Game with ID: {GameId} not found for deletion.", id);
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundById, id));
            }

            if (game.Genres != null)
            {
                logger.LogInformation("Removing genres for game with ID: {GameId}.", id);
                await repositoryFacade.GameGenreRepository.RemoveGameGenresAsync(game.Genres);
            }

            if (game.Platforms != null)
            {
                logger.LogInformation("Removing platforms for game with ID: {GameId}.", id);
                await repositoryFacade.GamePlatformRepository.RemoveGamePlatformsAsync(game.Platforms);
            }

            await repositoryFacade.GameRepository.DeleteAsync(game);

            logger.LogInformation("Game with ID: {GameId} deleted successfully.", id);

            return true;
        }

        public async Task<GameResponse?> GetGameByKeyAsync(string key)
        {
            var game = await repositoryFacade.GameRepository.GetByKeyAsync(key);

            if (game == null)
            {
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundByKey, key));
            }

            return new GameResponse
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description ?? ErrorMessages.NoDescription,
                Key = game.Key,
                Price = game.Price,
                UnitInStock = game.UnitInStock,
                Discount = game.Discount,
            };

        }

        public async Task<bool> DeleteGameByKeyAsync(string key)
        {
            logger.LogInformation("Deleting game with key: {GameKey}", key);

            var game = await repositoryFacade.GameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                logger.LogError("Game with key: {GameKey} not found for deletion.", key);
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundByKey, key));
            }

            if (game.Genres != null)
            {
                logger.LogInformation("Removing genres for game with key: {GameKey}.", key);
                await repositoryFacade.GameGenreRepository.RemoveGameGenresAsync(game.Genres);
            }

            if (game.Platforms != null)
            {
                logger.LogInformation("Removing platforms for game with key: {GameKey}.", key);
                await repositoryFacade.GamePlatformRepository.RemoveGamePlatformsAsync(game.Platforms);
            }

            await repositoryFacade.GameRepository.DeleteAsync(game);

            logger.LogInformation("Game with key: {GameKey} deleted successfully.", key);

            return true;
        }


       public async Task<IEnumerable<GameResponse?>> GetGamesByPlatformAsync(Guid platformId)
        {
            var games = await repositoryFacade.GameRepository.GetGamesByPlatformAsync(platformId);

            return games.Select(g =>
            {
                if (g != null)
                    return new GameResponse
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description ?? ErrorMessages.NoDescription,
                        Key = g.Key,
                        Price = g.Price,
                        UnitInStock = g.UnitInStock,
                        Discount = g.Discount,
                    };
                return null;
            });
        }

        public async Task<IEnumerable<GameResponse?>> GetGamesByGenreAsync(Guid genreId)
        {
            var games = await repositoryFacade.GameRepository.GetGamesByGenreAsync(genreId);
            return games.Select(g =>
            {
                if (g != null)
                    return new GameResponse
                    {
                        Id = g.Id,
                        Name = g.Name,
                        Description = g.Description ?? ErrorMessages.NoDescription,
                        Key = g.Key,
                        Price = g.Price,
                        UnitInStock = g.UnitInStock,
                        Discount = g.Discount,
                    };
                return null;
            });
        }
        private async Task<string> GenerateKeyFromName(string name)
        {
            var baseKey = name.ToLower().Replace(" ", "-");
            var similarKeys = await repositoryFacade.GameRepository.GetAllKeysStartingWithAsync(baseKey);
            if (similarKeys.Count == 0)
            {
                return baseKey;
            }
            var maxCounter = similarKeys
                .Where(key => key.Length > baseKey.Length)
                .Select(key => 
                {
                    var numberPart = key.Substring(baseKey.Length);
                    return int.TryParse(numberPart, out int num) ? num : 0;
                })
                .Max();
            return $"{baseKey}{maxCounter + 1}";
        }
        public async Task<string> DownloadGameByKeyAsync(string key)
        {
            logger.LogInformation("Downloading game with key: {GameKey}", key);
            var game = await repositoryFacade.GameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                logger.LogError("Game with key: {GameKey} not found for download.", key);
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundByKey, key));
            }
            var content = new StringBuilder();
            content.AppendLine("Id,Name,Description,Key,Price,UnitInStock,Discount");
            content.AppendLine($"{game.Id},{game.Name},{game.Description ?? ErrorMessages.NoDescription},{game.Key},{game.Price},{game.UnitInStock},{game.Discount}");
            return content.ToString();
        }
        public async Task<PublisherResponse?> GetPublisherByGameKeyAsync(string key)
        {
            var game = await repositoryFacade.GameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                throw new KeyNotFoundException(ErrorMessages.GameNotFoundByKey);
            }

            var publisher = await repositoryFacade.PublisherRepository.GetByIdAsync(game.PublisherId);
            if (publisher == null)
            {
                throw new KeyNotFoundException(ErrorMessages.PublisherNotFound);
            }

            return new PublisherResponse
            {
                Id          = publisher.Id,
                CompanyName = publisher.CompanyName,
                HomePage    = publisher.HomePage,
                Description = publisher.Description,
            };
        }
        public async Task<bool> BuyGameAsync(string key, Guid customerId)
        {
            logger.LogInformation("Processing game purchase request for key: {GameKey}, CustomerId: {CustomerId}", key, customerId);

            var game = await repositoryFacade.GameRepository.GetByKeyAsync(key);
            if (game == null)
            {
                logger.LogError("Game with key: {GameKey} not found.", key);
                throw new KeyNotFoundException(string.Format(ErrorMessages.GameNotFoundByKey, key));
            }

            if (game.UnitInStock <= 0)
            {
                logger.LogWarning("Game with key: {GameKey} is out of stock.", key);
                throw new InvalidOperationException(ErrorMessages.GameOutOfStock);
            }

            var cart = await repositoryFacade.OrderRepository.GetCartByCustomerIdAsync(customerId);
            if (cart == null)
            {
                cart = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    Date = DateTime.UtcNow,
                    Status = OrderStatus.Open,
                    OrderGames = new List<OrderGame>()
                };
                await repositoryFacade.OrderRepository.AddAsync(cart);
            }

            var existingOrderGame = cart.OrderGames.FirstOrDefault(og => og.ProductId == game.Id);
            if (existingOrderGame != null)
            {
                if (existingOrderGame.Quantity + 1 > game.UnitInStock)
                {
                    logger.LogWarning("Cannot add more games than available in stock.");
                    throw new InvalidOperationException(ErrorMessages.GameOutOfStock);
                }

                existingOrderGame.Quantity += 1;
            }
            else
            {
                cart.OrderGames.Add(new OrderGame
                {
                    OrderId = cart.Id,
                    ProductId = game.Id,
                    Price = game.Price,
                    Quantity = 1,
                    Discount = game.Discount
                });
            }

            await repositoryFacade.OrderRepository.UpdateAsync(cart);
            return true;
        }
        public async Task<PagedResultDto<GameResponse>> 
            GetFilteredGamesAsync(GameFilterRequest filter)
        {
            var query = (await repositoryFacade.GameRepository.GetAllAsync())
                .AsQueryable();

            var pipeline = new GamePipeline()
                .AddStep(new GenreFilterStep(filter.Genres))
                .AddStep(new PlatformFilterStep(filter.Platforms))
                .AddStep(new PublisherFilterStep(filter.Publishers))
                .AddStep(new PriceRangeFilterStep(filter.MinPrice, filter.MaxPrice))
                .AddStep(new PublishDateFilterStep(filter.PublishDateOption))
                .AddStep(new NameFilterStep(filter.NamePart))
                .AddStep(new SortingStep(filter.SortingOption));

            var countQuery   = pipeline.Execute(query);
            int totalItems   = countQuery.Count();

            var pagedPipeline = pipeline.AddStep(
                new PaginationStep<Game>(filter.Page, filter.PageSize)
            );
            var pagedQuery    = pagedPipeline.Execute(query);
            var games         = pagedQuery.ToList();

            var items = games.Select(g => new GameResponse {
                Id          = g.Id,
                Name        = g.Name,
                Description = g.Description ?? ErrorMessages.NoDescription,
                Key         = g.Key,
                Price       = g.Price,
                UnitInStock = g.UnitInStock,
                Discount    = g.Discount
            }).ToList();

            return new PagedResultDto<GameResponse> {
                Items      = items,
                TotalItems = totalItems,
                Page       = filter.Page,
                PageSize   = filter.PageSize
            };
        }


        public async Task UpdateStockAfterPurchaseAsync(IEnumerable<OrderGame> orderGames)
        {
            foreach (var orderGame in orderGames)
            {
                var game = await repositoryFacade.GameRepository.GetByIdAsync(orderGame.ProductId);
                game.UnitInStock -= orderGame.Quantity;
                if (game.UnitInStock < 0)
                {
                    game.UnitInStock = 0;
                }

                await repositoryFacade.GameRepository.UpdateAsync(game);
            }
        }
        
    }
}
