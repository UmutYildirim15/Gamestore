using Data.Entities;

namespace Business.Filtering.Steps
{
    public class NameFilterStep(string? namePart) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (string.IsNullOrWhiteSpace(namePart) || namePart.Length < 3)
                return input;
            
            return input.Where(game => game.Name.Contains(namePart));
        }
    }
}