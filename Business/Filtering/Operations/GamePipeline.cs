using Data.Entities;

namespace Business.Filtering.Operations
{
    public class GamePipeline
    {
        private readonly List<IPipelineStep<Game>> _steps = new();

        public GamePipeline AddStep(IPipelineStep<Game> step)
        {
            _steps.Add(step);
            return this;
        }

        public IQueryable<Game> Execute(IQueryable<Game> input)
        {
            return _steps.Aggregate(input, (current, step) => step.Process(current));
        }
    }
}