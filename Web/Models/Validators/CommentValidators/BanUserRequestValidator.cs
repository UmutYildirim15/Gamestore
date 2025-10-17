using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.CommentValidators
{
    public abstract class BanUserRequestValidator : AbstractValidator<BanUserRequest>
    {
        protected BanUserRequestValidator()
        {
            RuleFor(x => x.User)
                .NotEmpty()
                .WithMessage("User is required.");

            RuleFor(x => x.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.")
                .Must(duration =>
                {
                    var allowedDurations = new[] { "1 hour", "1 day", "1 week", "1 month", "permanent" };
                    return allowedDurations.Contains(duration.ToLower());
                })
                .WithMessage("Duration must be one of: '1 hour', '1 day', '1 week', '1 month', 'permanent'.");
        }
    }
}