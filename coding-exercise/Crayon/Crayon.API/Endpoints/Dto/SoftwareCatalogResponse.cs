using Crayon.API.Models;
using LanguageExt.ClassInstances;

namespace Crayon.API.Endpoints.Dto;


public record SoftwareCatalogResponse(IEnumerable<SoftwareCatalogItemResponse> Items, int TotalCount);

public record SoftwareCatalogItemResponse(string SoftwareId, string Name, string Version, string Vendor);