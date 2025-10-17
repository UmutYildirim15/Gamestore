using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.OrderValidators
{
    public class OrderCreateValidator : AbstractValidator<OrderCreate>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("A valid CustomerId must be provided.");

            RuleFor(x => x.OrderGames)
                .NotEmpty().WithMessage("At least one game must be included in the order.")
                .ForEach(game => game.SetValidator(new OrderGameCreateValidator()));
        }
    }

    public class OrderGameCreateValidator : AbstractValidator<OrderGameCreate>
    {
        public OrderGameCreateValidator()
        {
            RuleFor(x => x.ProductId)
                .NotEmpty().WithMessage("ProductId cannot be empty.")
                .NotEqual(Guid.Empty).WithMessage("A valid ProductId must be provided.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");

            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.");
        }
    }
}