using FluentValidation;
using LoginRequest = Web.Models.RequestModels.LoginRequest;

namespace Web.Models.Validators.RoleValidators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty().WithMessage("Login value is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}