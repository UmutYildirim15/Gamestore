using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.GenreValidators
{
    public class GenreRequestValidator : AbstractValidator<GenreRequest>
    {
        public GenreRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Genre name is required.");
            RuleFor(x => x.ParentGenreId).NotEmpty().WithMessage("Parent genre ID must be valid.")
                .When(x => x.ParentGenreId.HasValue);
        }
    }
}