using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Business.Services.Dto;
using Web.Models.CreateModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("publishers")]
    [ApiController]
    public class PublisherController(IPublisherService publisherService) : ControllerBase
    {
        [HttpGet("{companyName}/games")]
        public async Task<ActionResult<IList<GameResponse>>> GetGamesByPublisherAsync([FromRoute] string companyName)
        {
            var games = await publisherService.GetGamesByPublisherNameAsync(companyName);
            return games.Any() ? Ok(games) : NotFound();
        }
        
        [HttpPut("{companyName}")]
        public async Task<IActionResult> UpdatePublisher([FromRoute] string companyName, [FromBody] PublisherCreate request)
        {
            var result = await publisherService.UpdatePublisherAsync(companyName, request.Publisher.CompanyName, request.Publisher.HomePage, request.Publisher.Description);
            return result ? NoContent() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPublishers()
        {
            var publishers = await publisherService.GetAllPublishersAsync();
            return Ok(publishers);
        }

        [HttpGet("{companyName}")]
        public async Task<IActionResult> GetPublisherByName([FromRoute] string companyName)
        {
            var publisher = await publisherService.GetPublisherByNameAsync(companyName);
            return Ok(publisher);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreatePublisher([FromBody] PublisherCreate request)
        {
            var publisherId = await publisherService.CreatePublisherAsync(request.Publisher.CompanyName, request.Publisher.HomePage, request.Publisher.Description);
            return CreatedAtAction("GetPublisherByName", 
                new { companyName = request.Publisher.CompanyName }, 
                new { id = publisherId, companyName = request.Publisher.CompanyName });

        }
        
        [HttpDelete("{publisherId:guid}")]
        public async Task<IActionResult> DeletePublisher(Guid publisherId)
        {
            await publisherService.DeletePublisherAsync(publisherId);
            return NoContent();
        }
        
    }
}