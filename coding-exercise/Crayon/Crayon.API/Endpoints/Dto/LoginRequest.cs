namespace Crayon.API.Endpoints.Dto;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string Token);