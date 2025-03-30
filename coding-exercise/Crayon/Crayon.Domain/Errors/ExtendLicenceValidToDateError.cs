namespace Crayon.Domain.Errors;

public enum ExtendLicenceValidToDateError
{
    LicenceDoesNotExist,
    LicenceSubscriptionNotActive,
    DateMustBeInFuture
}