namespace Crayon.API.Endpoints.Dto;

public record CustomerAccountsResponse(string CustomerName, IEnumerable<AccountResponse> Accounts);
public record AccountResponse(int Id, string Name);