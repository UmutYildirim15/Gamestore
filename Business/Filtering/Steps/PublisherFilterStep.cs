using Data.Entities;

namespace Business.Filtering.Steps
{
    public class PublisherFilterStep(List<string>? publishers) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (publishers == null || publishers.Count == 0)
                return input;

            var publisherIds = publishers.Select(p => Guid.Parse(p));
            return input.Where(game => publisherIds.Contains(game.PublisherId));
        }
    }
}