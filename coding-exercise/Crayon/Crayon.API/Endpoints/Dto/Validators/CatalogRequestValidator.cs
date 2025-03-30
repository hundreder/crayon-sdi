using FluentValidation;

namespace Crayon.API.Endpoints.Dto.Validators;

public class CatalogRequestValidator : AbstractValidator<CatalogRequest>
{
    public CatalogRequestValidator()
    {
        RuleFor(x => x.NameLike)
            .MinimumLength(4)
            .When(x => !string.IsNullOrWhiteSpace(x.NameLike))
            .WithMessage("Name must be longer than 3 characters.");

        RuleFor(x => x.Skip)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Skip.HasValue)
            .WithMessage("Skip cannot be negative.");

        RuleFor(x => x.Take)
            .GreaterThan(0)
            .When(x => x.Take.HasValue)
            .WithMessage("Take must be greater than 0.")
            .LessThanOrEqualTo(50)
            .When(x => x.Take.HasValue)
            .WithMessage("Take must be less than 50.");
    }
}