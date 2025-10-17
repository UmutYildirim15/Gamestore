namespace Shared.Models;

public class EntityFilter<T>(IList<T> records, int totalNoLimit)
    where T : class
{
    public IList<T> Records { get; set; } = records;

    public int TotalNoLimit { get; set; } = totalNoLimit;

    public IList<string> MongoBlacklist { get; set; } = new List<string>();
}