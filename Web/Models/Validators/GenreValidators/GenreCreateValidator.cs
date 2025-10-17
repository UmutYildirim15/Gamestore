using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.GenreValidators
{
    public abstract class GenreCreateValidator : AbstractValidator<GenreCreate>
    {
        protected GenreCreateValidator()
        {
            RuleFor(x => x.Genre).SetValidator(new GenreRequestValidator());
        }
    }
}