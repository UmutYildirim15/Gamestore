using Business.Services.Dto;
using Shared.Dto;

namespace Northwind.Business.Services.Abstract;

public interface IMongoSupplierService
{
    Task<PublisherResponse> GetPublisherByIdAsync(string id, string culture);
    Task<IList<PublisherResponse>> GetAllPublishersAsync(string culture);
    Task<IList<GameResponse>> GetGamesByPublisherIdAsync(string id, string culture);
    Task DeletePublishersAsync(string? id);
}