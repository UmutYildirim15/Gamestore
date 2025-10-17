using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.RoleValidators
{
    public class CheckAccessRequestValidator : AbstractValidator<CheckAccessRequest>
    {
        public CheckAccessRequestValidator()
        {
            RuleFor(x => x.TargetPage).NotEmpty().WithMessage("Target page is required.");
        }
    }
}