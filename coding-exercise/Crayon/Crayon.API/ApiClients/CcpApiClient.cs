using Crayon.API.Models;
using Order = Crayon.Domain.Models.Order;
using LanguageExt;

namespace Crayon.API.ApiClients;

public interface ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog();
    public Task<Either<CreateOrderError, string>> SendOrder(Order order);
}

public class CcpApiClient(HttpClient httpClient) : ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog()
    {
        var softwareServices = new List<Software>
        {
            new(1, "Windows 11", "21H2", "Microsoft", 139.99m),
            new(2, "Microsoft Office 365", "2023", "Microsoft", 99.99m),
            new(3, "Adobe Premiere Pro", "23.1", "Adobe", 239.88m),
            new(4, "VMware Workstation", "17 Pro", "VMware", 199.99m),
            new(5, "Slack", "Desktop 4.29", "Slack Technologies", 20m),
            new(6, "Zoom", "5.15", "Zoom Video", 149.90m),
            new(7, "Visual Studio 2022", "17.6", "Microsoft", 250.00m),
            new(8, "Notion", "Windows App", "Notion Labs", 10m),
            new(9, "Postman", "10.12", "Postman, Inc.", 20m),
            new(10, "Microsoft Edge", "115.0.0", "Microsoft", 240m),
            new(11, "Firefox", "118.0", "Mozilla", 11m),
            new(12, "Chrome", "115.0", "Google", 32.1m),
            new(13, "Unity", "2022.3.6", "Unity Technologies", 399.00m),
            new(14, "MySQL Workbench", "8.0.34", "Oracle", 5m),
            new(15, "GitHub Desktop", "3.3.4", "GitHub", 60m),
            new(16, "Blender", "3.6.2", "Blender Foundation", 77m),
            new(17, "Figma", "Desktop", "Figma, Inc.", 144.00m),
            new(18, "JetBrains Rider", "2024.3", "JetBrains", 199.00m),
            new(19, "Docker Desktop", "4.15.1", "Docker, Inc.", 09m),
            new(20, "PyCharm", "2023.2", "JetBrains", 199.00m)
        };
        return softwareServices;
    }

    public async Task<Either<CreateOrderError, string>> SendOrder(Order order)
    {
        if(order.OrderItems.Count > 2)
            return CreateOrderError.SubmittingOrderToExternalProviderFailed;
        
        return $"CCP-{Guid.NewGuid()}";
    }
}