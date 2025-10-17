namespace Shared.Dto
{
    public class PaymentResponse
    {
        public string Status { get; set; } = string.Empty;
        public double Sum { get; set; }
    }
    public class BankPaymentResult
    {
        public byte[]? PdfBytes { get; set; }
        public string? FileName { get; set; }
    }
}