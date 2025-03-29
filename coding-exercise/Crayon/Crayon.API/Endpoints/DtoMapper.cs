using Crayon.API.Endpoints.Dto;
using Crayon.API.Models;
using Crayon.Domain.Models;

namespace Crayon.API.Endpoints;

public static class DtoMapper
{
    public static LoggedInUserResponse ToResponse(this User user) =>
        new(user.Id, user.Email, user.Name);
    
    public static SoftwareCatalogResponse ToResponse(this SoftwareCatalog software) => 
        new(software.Items.Select(ToResponse), software.TotalCount);

    private static SoftwareCatalogItemResponse ToResponse(this Software software) =>
        new(software.Id, software.Name, software.Version, software.Vendor);

    public static CustomerAccountsResponse ToResponse(this Customer customer) => 
        new CustomerAccountsResponse(customer.Name, customer.Accounts.Select(a => new AccountResponse(a.Id, a.Name)));
}