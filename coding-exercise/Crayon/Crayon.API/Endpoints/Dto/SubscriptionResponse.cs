using Crayon.Domain.Models;

namespace Crayon.API.Endpoints.Dto;

public record SubscriptionResponse(
    int SubscriptionId,
    int SoftwareId,
    string SoftwareName,
    SubscriptionStatus Status,
    IEnumerable<LicenceResponse> Licences
);