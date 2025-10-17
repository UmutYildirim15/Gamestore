namespace Web.Models.CreateModels
{
    public class PaymentCreate
    {
        public required string Method { get; set; }
        public PaymentCardModel? Model { get; set; }
    }

    public class PaymentCardModel
    {
        public string? Holder { get; set; }
        public string? CardNumber { get; set; }
        public int MonthExpire { get; set; }
        public int YearExpire { get; set; }
        public int Cvv2 { get; set; }
    }
}