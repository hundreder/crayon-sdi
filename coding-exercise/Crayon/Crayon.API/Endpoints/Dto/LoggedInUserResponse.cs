namespace Crayon.API.Endpoints.Dto;

public record LoggedInUserResponse(int UserId, string Email, string Name);