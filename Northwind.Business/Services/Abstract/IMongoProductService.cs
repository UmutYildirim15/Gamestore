using Business.Services.Dto;
using Data.Entities;
using Shared.Dto;
using Shared.Models;

namespace Northwind.Business.Services.Abstract;

public interface IMongoProductService
{
    Task<GameResponse> GetGameByKeyAsync(string key, string culture);
    Task<IEnumerable<GameResponse>> GetAllGamesAsync();
    Task<GameResponse> GetGameByIdAsync(string id, string culture);
    Task<IList<GenreResponse>> GetGenresByGameKeyAsync(string key);
    Task<IList<PlatformResponse>> GetPlatformsByGameKeyAsync(string key);
    Task<PublisherResponse> GetPublisherByGameKeyAsync(string key);
    Task<EntityFilter<GameResponse>> GetFilteredGamesAsync(GameFilter filter);
    Task DeleteGameAsync(string key);
    Task<Tuple<byte[], string>> DownloadAsync(string gameKey);
    Task SyncStockAfterPaymentAsync(IEnumerable<OrderGame> orderGames);
}