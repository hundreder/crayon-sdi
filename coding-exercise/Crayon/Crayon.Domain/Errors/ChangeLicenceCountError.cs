namespace Crayon.Domain.Errors;

public enum ChangeLicenceCountError
{
    LicenceDoesNotExist,
    LicenceSubscriptionNotActive,
    LicenceCountMustBeGreaterThanZero,
    NewLicenceCountCantBeSameAsExising
}