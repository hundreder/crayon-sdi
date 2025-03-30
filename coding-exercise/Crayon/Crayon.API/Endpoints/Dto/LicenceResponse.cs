namespace Crayon.API.Endpoints.Dto;

public record LicenceResponse(
    int LicenceId,
    DateTimeOffset ValidTo,
    int LicenceCount,
    string LicenceKey
);