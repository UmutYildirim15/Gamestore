using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.OrderValidators
{
    public class PurchaseRequestValidator : AbstractValidator<PurchaseRequest>
    {
        public PurchaseRequestValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("A valid UserId must be provided.");
        }
    }
}