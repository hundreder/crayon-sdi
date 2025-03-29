namespace Crayon.API.Models;

public record Software(int Id, string Name, string Version, string Vendor, decimal Price);

public record SoftwareCatalog(IList<Software> Items, int TotalCount);