namespace Crayon.API.Endpoints.Dto;

public record NewOrderRequest(IEnumerable<NewOrderItemRequest> ItemsToOrder);
public record NewOrderItemRequest(int SoftwareId, int LicenseCount, DateTime LicencedUntil);

public record NewOrderResponse(int OrderId);