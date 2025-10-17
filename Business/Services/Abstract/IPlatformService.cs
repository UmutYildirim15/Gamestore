
using Shared.Dto;

namespace Business.Services.Abstract
{
    public interface IPlatformService
    {
        Task<IEnumerable<PlatformResponse>> GetAllPlatformsAsync();
        Task<PlatformResponse> GetPlatformByIdAsync(Guid id);
        Task<IEnumerable<PlatformResponse?>> GetPlatformsByGameKeyAsync(string key);
        Task<Guid> CreatePlatformAsync(string type);
        Task<bool> UpdatePlatformAsync(Guid id, string type);
        Task<bool> DeletePlatformAsync(Guid id);
    }
}
