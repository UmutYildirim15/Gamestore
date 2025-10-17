using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.GameValidators
{
    public abstract class GameCreateValidator : AbstractValidator<GameCreate>
    {
        protected GameCreateValidator()
        {
            RuleFor(x => x.Game).SetValidator(new GameRequestValidator());
            RuleFor(x => x.Genres).NotEmpty().WithMessage("At least one genre must be provided.");
            RuleFor(x => x.Platforms).NotEmpty().WithMessage("At least one platform must be provided.");
            RuleFor(x => x.Publisher)
                .NotEqual(Guid.Empty)
                .WithMessage("A valid publisher must be provided.");
        }
    }
}