
using Business.Services.Dto;
using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IPublisherService
    {
        Task<Guid> CreatePublisherAsync(string companyName, string? homePage, string? description);
        Task<PublisherResponse> GetPublisherByNameAsync(string companyName);
        Task<IEnumerable<PublisherResponse>> GetAllPublishersAsync();
        Task<bool> UpdatePublisherAsync(string companyName, string? newCompanyName, string? homePage, string? description);
        Task DeletePublisherAsync(Guid id);
        Task<IEnumerable<GameResponse>> GetGamesByPublisherNameAsync(string companyName);
        Task<IEnumerable<GameResponse>> GetGamesByPublisherIdAsync(Guid publisherId);

    }
}