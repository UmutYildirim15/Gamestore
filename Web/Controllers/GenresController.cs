using Microsoft.AspNetCore.Mvc;
using Business.Services.Abstract;
using Web.Models.CreateModels;
using Web.Utilities;

namespace Web.Controllers
{
    [Route("genres")]
    [ApiController]
    public class GenresController(IGenreService genreService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await genreService.GetAllGenresAsync();
            return Ok(genres);
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateGenre([FromBody] GenreCreate request)
        {
            var genreId = await genreService.CreateGenreAsync(request.Genre.Name, request.Genre.ParentGenreName);
            return CreatedAtAction(nameof(GetGenreById), new { id = genreId }, new { id = genreId });
        }
        
        [HttpGet("{genreId:guid}")]
        public async Task<IActionResult> GetGenreById([FromRoute] Guid genreId)
        {
            var genre = await genreService.GetGenreByIdAsync(genreId);
            return genre != null ? Ok(genre) : NotFound();
        }

        [HttpPut("{genreId:guid}")]
        public async Task<IActionResult> UpdateGenre([FromRoute] Guid genreId, [FromBody] GenreCreate request)
        {
            var updated = await genreService.UpdateGenreAsync(genreId, request.Genre.Name);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{genreId:guid}")]
        public async Task<IActionResult> DeleteGenre([FromRoute] Guid genreId)
        {
            var deleted = await genreService.DeleteGenreAsync(genreId);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("key/{key}/genres")]
        public async Task<IActionResult> GetGenresByGameKey([FromRoute] string key)
        {
            var genres = await genreService.GetGenresByGameKeyAsync(key);
            return genres.Any() ? Ok(genres) : NotFound();
        }
        [HttpGet("{genreId:guid}/subgenres")]
        public async Task<IActionResult> GetGenresByParentId([FromRoute] Guid genreId)
        {
            var subGenres = await genreService.GetGenresByParentIdAsync(genreId);
            return Ok(subGenres);
        }
    }
}
