using Business.Services.Dto;

namespace Shared.Dto
{
    public class FilteredGameResultDto
    {
        public IEnumerable<GameResponse> Games { get; set; } = new List<GameResponse>();
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}