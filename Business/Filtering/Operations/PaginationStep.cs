namespace Business.Filtering.Operations
{
    public class PaginationStep<T>(int page, int pageSize) : IPipelineStep<T>
    {
        public IQueryable<T> Process(IQueryable<T> input)
        {
            if (pageSize <= 0)
                return input;

            return input.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}