
namespace Business.Filtering
{
    public interface IPipelineStep<T>
    {
        IQueryable<T> Process(IQueryable<T> input);
    }
}
