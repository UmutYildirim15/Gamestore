using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.OrderValidators
{
    public class PaymentCreateValidator : AbstractValidator<PaymentCreate>
    {
        public PaymentCreateValidator()
        {
            RuleFor(x => x.Method)
                .NotEmpty().WithMessage("Payment method is required.")
                .Must(method => method == "Bank" || method == "IBox terminal" || method == "Visa")
                .WithMessage("Invalid payment method.");

            When(x => x.Method == "Visa", () =>
            {
                RuleFor(x => x.Model).NotNull().WithMessage("Card details are required for Visa payment.");
                RuleFor(x => x.Model).SetValidator(new PaymentCardModelValidator()!);
            });
        }
    }

    public class PaymentCardModelValidator : AbstractValidator<PaymentCardModel>
    {
        public PaymentCardModelValidator()
        {
            RuleFor(x => x.Holder)
                .NotEmpty().WithMessage("Card holder name is required.")
                .MaximumLength(100).WithMessage("Card holder name cannot exceed 100 characters.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Card number is required.")
                .Matches(@"^\d{13,19}$").WithMessage("Card number must be between 13 and 19 digits.");

            RuleFor(x => x.MonthExpire)
                .InclusiveBetween(1, 12).WithMessage("Expiration month must be between 1 and 12.");

            RuleFor(x => x.YearExpire)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Year).WithMessage("Expiration year must be in the future.");

            RuleFor(x => x.Cvv2)
                .InclusiveBetween(100, 9999).WithMessage("CVV2 must be 3 or 4 digits.");
        }
    }
}