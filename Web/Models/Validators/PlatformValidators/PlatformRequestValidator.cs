using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.PlatformValidators
{
    public class PlatformRequestValidator : AbstractValidator<PlatformRequest>
    {
        public PlatformRequestValidator()
        {
            RuleFor(x => x.Type).NotEmpty().WithMessage("Platform type is required.");
        }
    }
}