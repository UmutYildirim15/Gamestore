using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Web.Models.RequestModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("comments")]
    [ApiController]
    public class CommentsController(ICommentService commentService) : ControllerBase
    {
        [HttpGet("ban/durations")]
        public async Task<IActionResult> GetBanDurations()
        {
            var durations = await commentService.GetBanDurationsAsync();
            return Ok(durations);
        }
        [HttpPost("ban")]
        public async Task<IActionResult> BanUser([FromBody] BanUserRequest request)
        {
            await commentService.BanUserAsync(request.User, request.Duration);
            return Ok();
        }
    }
}