namespace Web.Models.RequestModels;

public class PaymentRequest
{
    public required string Method { get; set; }
    public PaymentModel? Model { get; set; }
}

public class PaymentModel
{
    public string? Holder { get; set; }
    public string? CardNumber { get; set; }
    public int? MonthExpire { get; set; }
    public int? YearExpire { get; set; }
    public int? CVV2 { get; set; }
}