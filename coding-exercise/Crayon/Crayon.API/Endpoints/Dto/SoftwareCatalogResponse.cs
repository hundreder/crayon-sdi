using Crayon.API.Models;
using LanguageExt.ClassInstances;

namespace Crayon.API.Endpoints.Dto;

public record SoftwareCatalogItemResponse(string Id, string Name, string Version, string Vendor)
{
    public static SoftwareCatalogItemResponse Create(Software item) =>
        new SoftwareCatalogItemResponse(item.Id, item.Name, item.Version, item.Vendor);
}

public record SoftwareCatalogResponse(IEnumerable<SoftwareCatalogItemResponse> Items, int TotalCount)
{
    public static SoftwareCatalogResponse Create(SoftwareCatalog softwareCatalog) =>
        new(softwareCatalog.Items.Select(SoftwareCatalogItemResponse.Create), softwareCatalog.TotalCount);
}