namespace Crayon.Domain.Errors;

public enum ExtendLicenceValidToDateError
{
    LicenceDoesNotExist,
    DateMustBeInFuture
}