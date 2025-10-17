using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.RoleValidators
{
    public class RoleRequestValidator : AbstractValidator<RoleRequest>
    {
        public RoleRequestValidator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("Valid Role ID required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Role name required.");
        }
    }
}