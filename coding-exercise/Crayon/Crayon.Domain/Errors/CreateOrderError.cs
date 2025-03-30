namespace Crayon.Domain.Errors;

public enum CreateOrderError
{
    AccountNotFound,
    SoftwareNotFound,
    NoItemsInOrder,
    SubmittingOrderToExternalProviderFailed
}