using Business.Services.Dto;
using Data.Entities;
using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IGameService
    {
        Task<IEnumerable<GameResponse>> GetAllGamesAsync();

        Task<Guid> CreateGameAsync(GameCreateDto gameDto);
        Task<bool> UpdateGameAsync(Guid id, GameCreateDto gameDto);
        
        Task<GameResponse?> GetGameByIdAsync(Guid id);

        Task<bool> DeleteGameAsync(Guid id);
        Task<GameResponse?> GetGameByKeyAsync(string key);
        Task<bool> DeleteGameByKeyAsync(string key);
        Task<IEnumerable<GameResponse?>> GetGamesByPlatformAsync(Guid platformId);
        Task<IEnumerable<GameResponse?>> GetGamesByGenreAsync(Guid genreId);
        
        Task<string> DownloadGameByKeyAsync(string key);
        Task<PublisherResponse?> GetPublisherByGameKeyAsync(string key);
        Task<bool> BuyGameAsync(string key, Guid customerId);

        Task<PagedResultDto<GameResponse>> GetFilteredGamesAsync(GameFilterRequest filter);

        Task UpdateStockAfterPurchaseAsync(IEnumerable<OrderGame> orderGames);
    }
}