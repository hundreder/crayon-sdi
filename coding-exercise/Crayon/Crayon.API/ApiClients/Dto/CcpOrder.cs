namespace Crayon.API.ApiClients.Dto;

public record NewCcpOrderRequest(string ExternalOrderId, string AccountId, List<NewCcpOrderItemRequest> Items);

public record NewCcpOrderItemRequest(string SoftwareId, int Quantity);