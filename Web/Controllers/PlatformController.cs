using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Web.Models.CreateModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("platforms")]
    [ApiController]
    public class PlatformsController(IPlatformService platformService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllPlatforms()
        {
            var platforms = await platformService.GetAllPlatformsAsync();
            return Ok(platforms);
        }

        [HttpGet("{platformId:guid}")]
        public async Task<IActionResult> GetPlatformById(Guid platformId)
        {
            var platform = await platformService.GetPlatformByIdAsync(platformId);

            return Ok(new
            {
                id = platform.Id.ToString(),
                type = platform.Type
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlatform([FromBody] PlatformCreate request)
        {
            var platformId = await platformService.CreatePlatformAsync(request.Platform.Type);
            return CreatedAtAction(nameof(GetPlatformById), new { id = platformId }, new { id = platformId });
        }

        [HttpPut("{platformId:guid}")]
        public async Task<IActionResult> UpdatePlatform([FromRoute] Guid platformId, [FromBody] PlatformCreate request)
        {
            var updated = await platformService.UpdatePlatformAsync(platformId, request.Platform.Type);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{platformId:guid}")]
        public async Task<IActionResult> DeletePlatform([FromRoute] Guid platformId)
        {
            var deleted = await platformService.DeletePlatformAsync(platformId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("key/{key}/platforms")]
        public async Task<IActionResult> GetPlatformsByGameKey([FromRoute] string key)
        {
            var platforms = await platformService.GetPlatformsByGameKeyAsync(key);
            return platforms.Any() ? Ok(platforms) : NotFound();
        }
    }
}
