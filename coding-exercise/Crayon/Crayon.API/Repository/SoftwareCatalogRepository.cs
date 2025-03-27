using Crayon.API.ApiClients;
using Crayon.API.Models;
using LanguageExt;
using LanguageExt.SomeHelp;

namespace Crayon.API.Repository;

public interface ISoftwareCatalogRepository
{
    Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct);
    Task<IEnumerable<Software>> GetSoftware(IEnumerable<string> ids, CancellationToken ct);
}

public class SoftwareCatalogRepository(ICcpApiClient ccpApiClient) : ISoftwareCatalogRepository
{
    public async Task<SoftwareCatalog> GetSoftwareCatalog(string? nameLike, int? skip, int? take, CancellationToken ct)
    {
        var entireCatalog = ccpApiClient.GetEntireCatalog().ToList();

        var filteredItems = entireCatalog
            .Where(ss => nameLike == null || ss.Name.Contains(nameLike, StringComparison.InvariantCultureIgnoreCase))
            .Skip(skip ?? 0)
            .Take(take ?? 100)
            .ToList();

        return new(filteredItems, entireCatalog.Count);
    }

    public async Task<IEnumerable<Software>> GetSoftware(IEnumerable<string> ids, CancellationToken ct) =>
        ccpApiClient.GetEntireCatalog()
            .Where(s => ids.Contains(s.Id));
}