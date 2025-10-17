using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.UserValidators
{
    public class UserRequestValidator : AbstractValidator<UserRequest>
    {
        public UserRequestValidator()
        {
            RuleFor(x => x.Id).NotEqual(Guid.Empty).WithMessage("A valid ID is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }
}