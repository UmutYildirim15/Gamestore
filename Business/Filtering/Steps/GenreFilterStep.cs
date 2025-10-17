using Data.Entities;

namespace Business.Filtering.Steps
{
    public class GenreFilterStep(List<string>? genres) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (genres == null || genres.Count == 0)
                return input;

            var lowerGenres = genres.Select(g => g.ToLowerInvariant()).ToList();
            return input.Where(game => game.Genres != null &&
                                       game.Genres.Any(gg => gg.Genre != null &&
                                                             lowerGenres.Contains(gg.Genre.Name.ToLowerInvariant())));

        }
    }
}