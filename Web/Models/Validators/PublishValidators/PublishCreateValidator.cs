using FluentValidation;
using Web.Models.CreateModels;

namespace Web.Models.Validators.PublishValidators
{
    public class PublisherCreateValidator : AbstractValidator<PublisherCreate>
    {
        public PublisherCreateValidator()
        {
            RuleFor(x => x.Publisher).SetValidator(new PublishRequestValidator());

        }
    }
}