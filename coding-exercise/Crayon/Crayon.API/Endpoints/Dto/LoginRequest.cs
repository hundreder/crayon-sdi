namespace Crayon.API.Endpoints.Dto;

public record LoginRequest(string Email);

public record LoginResponse(string Token);