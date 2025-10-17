using Data.Entities;

namespace Business.Filtering.Steps
{
    public class PlatformFilterStep(List<string>? platforms) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (platforms == null || platforms.Count == 0)
                return input;

            var lowerPlatforms = platforms.Select(p => p.ToLowerInvariant()).ToList();

            return input.Where(game => game.Platforms != null &&
                                       game.Platforms.Any(gp => gp.Platform != null &&
                                                                lowerPlatforms.Contains(gp.Platform.Type.ToLowerInvariant())));
        }
    }
}