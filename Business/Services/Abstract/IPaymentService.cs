using Shared.Dto;

namespace Business.Services.Abstract;

public interface IPaymentService
{
    Task<PaymentMethodsResponse> GetPaymentMethodsAsync();

    Task<object?> ProcessPaymentAsync(string method, string? cardHolder = null,
        string? cardNumber = null, int? monthExpire = null, int? yearExpire = null, int? cvv2 = null);
}