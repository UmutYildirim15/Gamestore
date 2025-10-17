using Shared.Util;

namespace Shared.Models;

public class OrderFilter
{
    public OrderFilter(DateTime? lowerDate, DateTime? upperDate)
    {
        LowerDate = lowerDate;
        UpperDate = upperDate;
    }
    public IList<string> Stores { get; set; } = new List<string> { StoreType.GameStore, StoreType.Northwind };

    public DateTime? LowerDate { get; set; } = DateTime.MinValue;

    public DateTime? UpperDate { get; set; } = DateTime.MaxValue;

    public string? CustomerId { get; set; }

    public bool OnlyPaid { get; set; }
}