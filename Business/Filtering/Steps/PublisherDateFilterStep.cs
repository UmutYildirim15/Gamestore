using System;
using System.Linq;
using Data.Entities;

namespace Business.Filtering
{
    public class PublishDateFilterStep(string? publishDateOption) : IPipelineStep<Game>
    {
        public IQueryable<Game> Process(IQueryable<Game> input)
        {
            if (string.IsNullOrWhiteSpace(publishDateOption))
                return input;

            DateTime threshold;
            switch (publishDateOption.ToLower())
            {
                case "last week":
                    threshold = DateTime.UtcNow.AddDays(-7);
                    break;
                case "last month":
                    threshold = DateTime.UtcNow.AddMonths(-1);
                    break;
                case "last year":
                    threshold = DateTime.UtcNow.AddYears(-1);
                    break;
                case "2 years":
                    threshold = DateTime.UtcNow.AddYears(-2);
                    break;
                case "3 years":
                    threshold = DateTime.UtcNow.AddYears(-3);
                    break;
                default:
                    return input;
            }
            return input.Where(game => game.CreatedDate >= threshold);
        }
    }
}