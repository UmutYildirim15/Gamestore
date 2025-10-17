using System.Net.Http.Json;
using System.Text.Json;
using Business.Services.Abstract;
using Business.Services.Resources;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.Extensions.Logging;
using Data.Entities;
using Data.Repositories.Abstract;
using System.Transactions;
using Microsoft.Extensions.Configuration;
using Shared.Dto;

namespace Business.Services.Concrete
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PaymentService> _logger;
        private readonly HttpClient _httpClient;
        private readonly int _invoiceValidityDays;
        private readonly Guid _stubCustomerId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f69afa2");

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public PaymentService(IOrderRepository orderRepository, ILogger<PaymentService> logger, HttpClient httpClient, IConfiguration configuration)
        {
            _orderRepository = orderRepository;
            _logger = logger;
            _httpClient = httpClient;
            var paymentApiBaseUrl = configuration["PaymentApiBaseUrl"] ?? throw new InvalidOperationException("Payment API base URL is not configured.");
            httpClient.BaseAddress = new Uri(paymentApiBaseUrl);
            _invoiceValidityDays = Convert.ToInt32(configuration["InvoiceSettings:ValidityDays"]);
        }

        public async Task<PaymentMethodsResponse> GetPaymentMethodsAsync()
        {
            return await Task.FromResult(new PaymentMethodsResponse
            {
                PaymentMethods =
                [
                    new PaymentMethod
                        { ImageUrl = "image1.png", Title = "Bank", Description = "Bank transfer payment" },
                    new PaymentMethod
                        { ImageUrl = "image2.png", Title = "IBox terminal", Description = "Terminal payment" },
                    new PaymentMethod { ImageUrl = "image3.png", Title = "Visa", Description = "Card payment" }
                ]
            });
        }
    
        public async Task<object?> ProcessPaymentAsync(string method, string? cardHolder = null, string? cardNumber = null, int? monthExpire = null, int? yearExpire = null, int? cvv2 = null)
        {
            _logger.LogInformation("Processing payment with method: {Method}", method);
            var cart = await _orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            if (cart == null)
                throw new InvalidOperationException("Cart is empty.");
            
            double sum = cart.OrderGames.Sum(g => g.Price * g.Quantity * (1 - ((g.Discount ?? 0) / 100.0)));
            DateTime paymentDate = DateTime.UtcNow;

            switch (method)
            {
                case "Bank":
                    return await ProcessBankPaymentAsync(paymentDate, sum);
                case "IBox terminal":
                    return await ProcessIBoxPaymentAsync(sum);
                case "Visa":
                    ValidateVisaPaymentDetails(cardHolder, cardNumber, monthExpire, yearExpire, cvv2);
                    return await ProcessVisaPaymentAsync(sum, cardHolder!, cardNumber!, monthExpire!.Value, yearExpire!.Value, cvv2!.Value);
                default:
                    throw new InvalidOperationException("Invalid payment method");
            }
        }

        private async Task<BankPaymentResult> ProcessBankPaymentAsync(DateTime paymentDate, double sum)
        {
            var cart = await _orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            if (cart != null)
            {
                byte[] invoicePdf = GeneratePdf(_stubCustomerId, cart.Id, paymentDate, sum);
        
                using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    cart.Status = OrderStatus.Paid;
                    await _orderRepository.UpdateAsync(cart);
                    transaction.Complete();
                }

                return new BankPaymentResult { PdfBytes = invoicePdf, FileName = "invoice.pdf" };
            }

            return new BankPaymentResult { PdfBytes = null, FileName = string.Empty };
        }

        private async Task<PaymentResponse?> ProcessIBoxPaymentAsync(double sum)
        {
            var cart = await _orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            var iboxPayload = new { method = "IBox terminal" };

            var iboxResponse = await AttemptPaymentAsync(() =>
                _httpClient.PostAsJsonAsync("api/payments/ibox", iboxPayload), "IBox terminal", sum);
            
            if (iboxResponse != null)
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                if (cart != null)
                {
                    cart.Status = OrderStatus.Paid;
                    await _orderRepository.UpdateAsync(cart);
                }

                transaction.Complete();
                return iboxResponse;
            }
            
            _logger.LogWarning("IBox terminal payment failed. Consider alternative payment method.");
            return null;
        }

        private static void ValidateVisaPaymentDetails(string? cardHolder, string? cardNumber, int? monthExpire, int? yearExpire, int? cvv2)
        {
            if (string.IsNullOrWhiteSpace(cardHolder) || string.IsNullOrWhiteSpace(cardNumber) ||
                !monthExpire.HasValue || !yearExpire.HasValue || !cvv2.HasValue)
            {
                throw new InvalidOperationException("Visa payment requires complete card details.");
            }
        }

        private async Task<PaymentResponse?> ProcessVisaPaymentAsync(double sum, string cardHolder, string cardNumber, int monthExpire, int yearExpire, int cvv2)
        {
            var cart = await _orderRepository.GetCartByCustomerIdAsync(_stubCustomerId);
            var visaPayload = new
            {
                cardHolderName = cardHolder,
                cardNumber,
                expirationMonth = monthExpire,
                expirationYear = yearExpire,
                cvv = cvv2
            };

            var visaResponse = await AttemptPaymentAsync(() =>
                _httpClient.PostAsJsonAsync("api/payments/visa", visaPayload), "Visa", sum);
            
            if (visaResponse != null)
            {
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                if (cart != null)
                {
                    cart.Status = OrderStatus.Paid;
                    await _orderRepository.UpdateAsync(cart);
                }

                transaction.Complete();
                return visaResponse;
            }
            
            _logger.LogWarning("Visa payment failed. Consider alternative payment method.");
            return null;
        }

        private async Task<PaymentResponse?> AttemptPaymentAsync(Func<Task<HttpResponseMessage>> paymentRequest, string paymentMethod, double sum)
        {
            try
            {
                var response = await paymentRequest();
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Request failed with status code: {StatusCode} for {PaymentMethod}.", response.StatusCode, paymentMethod);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var paymentResponse = JsonSerializer.Deserialize<PaymentResponse>(json, JsonSerializerOptions);
                if (paymentResponse == null)
                {
                    _logger.LogWarning("Payment response is empty for {PaymentMethod}.", paymentMethod);
                    return null;
                }

                if (paymentResponse.Status == "Paid")
                {
                    return paymentResponse;
                }
                else if (paymentResponse.Status == "Cancelled")
                {
                    if (ShouldRetry(paymentResponse, sum))
                    {
                        _logger.LogWarning("Payment failed for {PaymentMethod} but within tolerance limits. Retrying...", paymentMethod);
                        return await AttemptPaymentAsync(paymentRequest, paymentMethod, sum);
                    }
                }
                else
                {
                    _logger.LogWarning("Payment failed for {PaymentMethod}: {ErrorMessage}", paymentMethod, ErrorMessages.UnsuccessfulPayment);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during {PaymentMethod} payment attempt: {ErrorMessage}", paymentMethod, ex.Message);
                throw;
            }
            return null;
        }

        private static bool ShouldRetry(PaymentResponse paymentResponse, double sum)
        {
            var toleranceDifference = Math.Abs(paymentResponse.Sum - sum);
            return toleranceDifference <= sum * 0.1;
        }

        private byte[] GeneratePdf(Guid userId, Guid orderId, DateTime creationDate, double sum)
        {
            using var ms = new MemoryStream();
            var writerProperties = new WriterProperties();
            var writer = new PdfWriter(ms, writerProperties);
            using (var pdfDoc = new PdfDocument(writer))
            {
                using (var document = new Document(pdfDoc))
                {
                    document.Add(new Paragraph($"User ID: {userId}"));
                    document.Add(new Paragraph($"Order ID: {orderId}"));
                    document.Add(new Paragraph($"Creation Date: {creationDate}"));
                    document.Add(new Paragraph($"Valid Until: {creationDate.AddDays(_invoiceValidityDays)}"));
                    document.Add(new Paragraph($"Sum: {sum:C}"));
                    document.Close();
                }
            }
            var result = ms.ToArray();
            return result;
        }
    }
}
