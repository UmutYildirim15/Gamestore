using FluentValidation;
using Web.Models.RequestModels;

namespace Web.Models.Validators.PublishValidators
{
    public class PublishRequestValidator : AbstractValidator<PublisherRequest>
    {
        public PublishRequestValidator()
        {
            RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Company Name is required.");
        }
    }
}