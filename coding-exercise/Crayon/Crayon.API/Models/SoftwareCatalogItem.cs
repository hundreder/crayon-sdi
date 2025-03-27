namespace Crayon.API.Models;

public record SoftwareCatalogItem(int Id, string Name, string Version, string Vendor);

public record SoftwareCatalog(IList<SoftwareCatalogItem> Items, int TotalCount);