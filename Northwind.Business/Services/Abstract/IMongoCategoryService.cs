using Business.Services.Dto;
using Shared.Dto;

namespace Northwind.Business.Services.Abstract;

public interface IMongoCategoryService
{
    Task<Shared.Dto.GenreResponse> GetGenreByIdAsync(string id, string culture);
    Task<IList<GenreResponse>> GetAllGenresAsync(string culture);
    Task<IList<GenreResponse>> GetSubgenresByParentAsync(string parentId, string culture);
    Task<IList<GameResponse>> GetGamesByGenreIdAsync(string id, string culture);
    Task DeleteGenreAsync(string? genreId);
}