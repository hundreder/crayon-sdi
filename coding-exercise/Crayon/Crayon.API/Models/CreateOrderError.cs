namespace Crayon.API.Models;

public enum CreateOrderError
{
    AccountNotFound,
    SoftwareNotFound,
    SubmittingOrderToExternalProviderFailed
}