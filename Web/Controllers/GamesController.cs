using System.Text;
using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Business.Services.Dto;
using Shared.Dto;
using Web.Models.CreateModels;
using Web.Models.RequestModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("games")]
    [ApiController]
    public class GamesController(IGameService gameService, ICommentService commentService) : ControllerBase
    {
        [HttpGet("all")]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await gameService.GetAllGamesAsync();
            return Ok(games);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetAllGames([FromQuery] GameQuery queryParams)
        {
            // Debug information to check what's being received
            Console.WriteLine($"Genres count: {queryParams.Genres?.Count ?? 0}");
            Console.WriteLine($"Platforms count: {queryParams.Platforms?.Count ?? 0}");
            Console.WriteLine($"Publishers count: {queryParams.Publishers?.Count ?? 0}");

            var gameDto = new GameFilterRequest()
            {
                Genres = queryParams.Genres,
                Platforms = queryParams.Platforms,
                Publishers = queryParams.Publishers,
                MinPrice = queryParams.MinPrice,
                MaxPrice = queryParams.MaxPrice,
                PublishDateOption = queryParams.PublishDateOption,
                NamePart = queryParams.NamePart,
                SortingOption = queryParams.SortingOption,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
    
            var filteredResult = await gameService.GetFilteredGamesAsync(gameDto);
            return Ok(filteredResult);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGame([FromBody] GameCreate request)
        {
            if (request.Game.Description == null) return NoContent();
            var gameDto = new GameCreateDto
            {
                Name = request.Game.Name,
                Description = request.Game.Description,
                Key = request.Game.Key,
                Price = request.Game.Price,
                UnitInStock = request.Game.UnitInStock,
                Discount = request.Game.Discount,
                GenreIds = request.Genres,
                PlatformIds = request.Platforms,
                PublisherId = request.Publisher
            };

            var gameId = await gameService.CreateGameAsync(gameDto);
            return CreatedAtAction(nameof(GetGameById), new { id = gameId }, new { id = gameId });
        }

        
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetGameById(Guid id)
        {
            var game = await gameService.GetGameByIdAsync(id);
            return game != null ? Ok(game) : NotFound();
        }
        
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateGame([FromRoute] Guid id, [FromBody] GameCreate request)
        {
            if (string.IsNullOrWhiteSpace(request.Game.Description))
            {
                return BadRequest("Game details with a valid description are required.");
            }

            var gameDto = new GameCreateDto
            {
                Name = request.Game.Name,
                Description = request.Game.Description,
                Key = request.Game.Key,
                Price = request.Game.Price,
                UnitInStock = request.Game.UnitInStock,
                Discount = request.Game.Discount,
                GenreIds = request.Genres,
                PlatformIds = request.Platforms,
                PublisherId = request.Publisher
            };

            var updated = await gameService.UpdateGameAsync(id, gameDto);
            return updated ? NoContent() : NotFound();
        }
        
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteGame([FromRoute] Guid id)
        {
            var deleted = await gameService.DeleteGameAsync(id);
            return deleted ? NoContent() : NotFound();
        }


        [HttpGet("platforms/{platformId:guid}")]
        public async Task<IActionResult> GetGamesByPlatform(Guid platformId)
        {
            var games = await gameService.GetGamesByPlatformAsync(platformId);
            return games.Any() ? Ok(games) : NotFound();
        }

        [HttpGet("genres/{genreId:guid}")]
        public async Task<IActionResult> GetGamesByGenre(Guid genreId)
        {
            var games = await gameService.GetGamesByGenreAsync(genreId);
            return games.Any() ? Ok(games) : NotFound();
        }
        [HttpGet("{key}")]
        public async Task<IActionResult> GetGameByKey(string key)
        {
            var game = await gameService.GetGameByKeyAsync(key);
            return game != null ? Ok(game) : NotFound();
        }

        [HttpDelete("key/{key}")]
        public async Task<IActionResult> DeleteGameByKey(string key)
        {
            var deleted = await gameService.DeleteGameByKeyAsync(key);
            return deleted ? NoContent() : NotFound();
        }
        
        [HttpGet("{key}/file")]
        public async Task<IActionResult> DownloadGameFile([FromRoute] string key) 
        {
            var game = await gameService.DownloadGameByKeyAsync(key);
            return File(Encoding.UTF8.GetBytes(game), "text/plain", $"{key}_game_info.txt");
        }
        
        [HttpGet("{key}/publisher")]
        public async Task<IActionResult> GetPublisherByGameKey([FromRoute] string key)
        {
            var publisher = await gameService.GetPublisherByGameKeyAsync(key);
            return publisher != null ? Ok(publisher) : NotFound();
        }
        [HttpPost("{key}/buy")]
        public async Task<IActionResult> BuyGame([FromRoute] string key, [FromBody] PurchaseRequest request)
        {
            var success = await gameService.BuyGameAsync(key, request.UserId);
            return success ? Ok() : BadRequest("Could not process the purchase.");
        }
        
        [HttpPost("{key}/comments")]
        public async Task<IActionResult> AddComment(string key, [FromBody] CommentCreate request)
        {
            var commentResponse = await commentService.AddCommentAsync(key, request.Name,request.Body,request.ParentId,request.Action);
            return Ok(commentResponse);
        }
        [HttpGet("{key}/comments")]
        public async Task<IActionResult> GetComments(string key)
        {
            var comments = await commentService.GetCommentsByGameKeyAsync(key);
            return Ok(comments);
        }

        [HttpDelete("{key}/comments/{id:guid}")]
        public async Task<IActionResult> DeleteComment(string key, Guid id)
        {
            await commentService.DeleteCommentAsync(key, id);
            return NoContent();
        }
        
        [HttpGet("pagination-options")]
        public IActionResult GetPaginationOptions()
        {
            var options = new[] { "10", "20", "50", "100", "all" };
            return Ok(options);
        }

        [HttpGet("sorting-options")]
        public IActionResult GetSortingOptions()
        {
            var options = new[] { "Most popular", "Most commented", "Price ASC", "Price DESC", "New" };
            return Ok(options);
        }

        [HttpGet("publish-date-options")]
        public IActionResult GetPublishDateOptions()
        {
            var options = new[] { "last week", "last month", "last year", "2 years", "3 years" };
            return Ok(options);
        }
    }
}
