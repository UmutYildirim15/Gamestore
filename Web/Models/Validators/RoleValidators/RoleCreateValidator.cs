using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.RoleValidators
{
    public class RoleCreateValidator : AbstractValidator<RoleCreate>
    {
        public RoleCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Rol name required.");
            RuleFor(x => x.Permissions).NotEmpty().WithMessage("At least one permission is required.");
        }
    }
}