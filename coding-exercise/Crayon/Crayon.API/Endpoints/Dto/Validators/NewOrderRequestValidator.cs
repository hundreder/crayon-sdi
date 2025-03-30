using FluentValidation;

namespace Crayon.API.Endpoints.Dto.Validators;


public class NewOrderRequestValidator : AbstractValidator<NewOrderRequest>
{
    public NewOrderRequestValidator()
    {
        RuleFor(x => x.ItemsToOrder)
            .NotNull().WithMessage("ItemsToOrder must not be null.")
            .Must(items => items != null && items.Any())
            .WithMessage("ItemsToOrder must contain at least one item.");
        
        RuleForEach(x => x.ItemsToOrder)
            .SetValidator(new NewOrderItemRequestValidator());
    }
}

public class NewOrderItemRequestValidator : AbstractValidator<NewOrderItemRequest>
{
    public NewOrderItemRequestValidator()
    {
        RuleFor(x => x.LicenseCount)
            .GreaterThan(0)
            .WithMessage("LicenseCount must be greater than zero.");

        RuleFor(x => x.LicencedUntil)
            .GreaterThan(DateTime.Today)
            .WithMessage("LicencedUntil must be a future date.");
    }
}
