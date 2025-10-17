using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.UserValidators
{
    public class UserCreateValidator : AbstractValidator<UserCreate>
    {
        public UserCreateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.RoleIds)
                .NotEmpty().WithMessage("At least one role is required.");
        }
    }
}