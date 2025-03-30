using Crayon.API.Endpoints.Dto;
using Crayon.Services.Models;

namespace Crayon.API.Endpoints;

public static class ModelMapper
{
    public static NewOrder ToModel(this NewOrderRequest newOrderRequest, int customerId, int accountId)
    {
        var items = newOrderRequest
            .ItemsToOrder
            .Select(i => new NewOrderItem(i.SoftwareId, i.LicenseCount, i.LicencedUntil))
            .ToList();

        return new NewOrder(customerId, accountId, items);
    }
    
}