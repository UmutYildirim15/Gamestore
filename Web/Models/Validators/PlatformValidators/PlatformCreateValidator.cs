using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.PlatformValidators
{
    public abstract class PlatformCreateValidator : AbstractValidator<PlatformCreate>
    {
        protected PlatformCreateValidator()
        {
            RuleFor(x => x.Platform).SetValidator(new PlatformRequestValidator());
        }
    }
}