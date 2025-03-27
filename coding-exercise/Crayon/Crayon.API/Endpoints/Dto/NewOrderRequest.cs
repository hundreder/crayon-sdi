namespace Crayon.API.Endpoints.Dto;

public record NewOrderRequest(IEnumerable<NewOrderItemRequest> ItemsToOrder);
public record NewOrderItemRequest(string SoftwareId, int LicenseCount, DateTime LicencedUntil);

public record NewOrderResponse(string OrderId);