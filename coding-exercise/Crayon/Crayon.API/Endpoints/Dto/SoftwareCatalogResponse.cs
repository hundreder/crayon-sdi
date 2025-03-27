using Crayon.API.Models;

namespace Crayon.API.Endpoints.Dto;

public record SoftwareCatalogItemResponse(int Id, string Name, string Version, string Vendor)
{
    public static SoftwareCatalogItemResponse Create(SoftwareCatalogItem item) =>
        new SoftwareCatalogItemResponse(item.Id, item.Name, item.Version, item.Vendor);
}

public record SoftwareCatalogResponse(IList<SoftwareCatalogItemResponse> Items, int TotalCount)
{
    public static SoftwareCatalogResponse Create(SoftwareCatalog softwareCatalog) =>
        new(softwareCatalog.Items.Select(SoftwareCatalogItemResponse.Create).ToList(), softwareCatalog.TotalCount);
}