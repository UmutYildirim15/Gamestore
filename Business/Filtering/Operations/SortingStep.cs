using Data.Entities;

namespace Business.Filtering.Operations
{
    public class SortingStep(string? sortingOption) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            return sortingOption switch
            {
                "Most popular" => input.OrderByDescending(game => game.ViewCount),
                "Most commented" => input.OrderByDescending(game => game.CommentCount),
                "Price ASC" => input.OrderBy(game => game.Price),
                "Price DESC" => input.OrderByDescending(game => game.Price),
                "New" => input.OrderByDescending(game => game.CreatedDate),
                _ => input
            };
        }
    }
}