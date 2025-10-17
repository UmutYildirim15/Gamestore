using Data.Entities;

namespace Business.Filtering.Steps
{
    public class PriceRangeFilterStep(double? minPrice, double? maxPrice) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (minPrice.HasValue)
                input = input.Where(game => game.Price >= minPrice.Value);
            if (maxPrice.HasValue)
                input = input.Where(game => game.Price <= maxPrice.Value);
            return input;
        }
    }
}