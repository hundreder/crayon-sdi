using Crayon.Domain.Models;
using Crayon.Repository;

namespace Crayon.Services.Services;

public interface ISoftwareCatalogService
{
    public Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct);
    public Task<IEnumerable<Software>> GetSoftware(List<int> ids, CancellationToken ct);
}

public class SoftwareCatalogService(ISoftwareCatalogRepository softwareCatalogRepository) : ISoftwareCatalogService
{
    public Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct) =>
        softwareCatalogRepository.GetSoftwareCatalog(nameLike, skip, take, ct);

    public Task<IEnumerable<Software>> GetSoftware(List<int> ids, CancellationToken ct) =>
        softwareCatalogRepository.GetSoftware(ids, ct);
}