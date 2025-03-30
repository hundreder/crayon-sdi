namespace Crayon.API.Endpoints.Dto;

public record CatalogRequest(string? NameLike, int? Skip, int? Take);