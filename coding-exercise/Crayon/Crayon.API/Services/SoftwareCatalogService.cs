using Crayon.API.Models;
using Crayon.API.Repository;

namespace Crayon.API.Services;

public interface ISoftwareCatalogService
{
    public Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct);
}

public class SoftwareCatalogService(ISoftwareCatalogRepository softwareCatalogRepository) : ISoftwareCatalogService
{
    public Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct) =>
        softwareCatalogRepository.GetSoftwareCatalog(nameLike, skip, take, ct);
}