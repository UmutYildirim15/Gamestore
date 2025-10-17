using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.GameValidators
{
    public class GameRequestValidator : AbstractValidator<GameRequest>
    {
        public GameRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Game name is required.");
            RuleFor(x => x.Key)
                .NotEmpty().WithMessage("Game key is required.");
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.UnitInStock)
                .GreaterThanOrEqualTo(0).WithMessage("UnitInStock cannot be negative.");
            RuleFor(x => x.Discount)
                .InclusiveBetween(0, 100).WithMessage("Discount must be between 0 and 100.");
        }
    }
}