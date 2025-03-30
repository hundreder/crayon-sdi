namespace Crayon.API.Endpoints.Dto;

public record LicenceResponse(string SoftwareName, string LicenceKey, DateTimeOffset ValidUntil);
