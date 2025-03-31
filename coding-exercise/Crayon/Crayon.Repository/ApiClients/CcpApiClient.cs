using Crayon.Domain.Errors;
using Crayon.Domain.Models;
using LanguageExt;

namespace Crayon.Repository.ApiClients;

public interface ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog();
    public Task<Either<CreateOrderError, CcpOrder>> SendOrder(Order order);
}

public class CcpApiClient(HttpClient httpClient) : ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog()
        => GetSoftwareServices();


    public async Task<Either<CreateOrderError, CcpOrder>> SendOrder(Order order)
    {
        if (order.OrderItems.Count > 2)
            return CreateOrderError.SubmittingOrderToExternalProviderFailed;

        var ccpOrder = new CcpOrder(
            $"CcpId-{Guid.NewGuid()}",
            order.OrderItems
                .Select(oi => new SoftwareLicence(
                    GetSoftware(oi.SoftwareId),
                    $"key-{Guid.NewGuid()}",
                    oi.LicenceCount,
                    oi.LicenceValidTo))
        );

        return ccpOrder;
    }

    private static Software GetSoftware(int id) => GetSoftwareServices().Single(s => s.Id == id);

    private static IEnumerable<Software> GetSoftwareServices()
    {
        yield return new(1, "Windows 11", "21H2", "Microsoft", 139.99m);
        yield return new(2, "Microsoft Office 365", "2023", "Microsoft", 99.99m);
        yield return new(3, "Adobe Premiere Pro", "23.1", "Adobe", 239.88m);
        yield return new(4, "VMware Workstation", "17 Pro", "VMware", 199.99m);
        yield return new(5, "Slack", "Desktop 4.29", "Slack Technologies", 20m);
        yield return new(6, "Zoom", "5.15", "Zoom Video", 149.90m);
        yield return new(7, "Visual Studio 2022", "17.6", "Microsoft", 250.00m);
        yield return new(8, "Notion", "Windows App", "Notion Labs", 10m);
        yield return new(9, "Postman", "10.12", "Postman, Inc.", 20m);
        yield return new(10, "Microsoft Edge", "115.0.0", "Microsoft", 240m);
        yield return new(11, "Firefox", "118.0", "Mozilla", 11m);
        yield return new(12, "Chrome", "115.0", "Google", 32.1m);
        yield return new(13, "Unity", "2022.3.6", "Unity Technologies", 399.00m);
        yield return new(14, "MySQL Workbench", "8.0.34", "Oracle", 5m);
        yield return new(15, "GitHub Desktop", "3.3.4", "GitHub", 60m);
        yield return new(16, "Blender", "3.6.2", "Blender Foundation", 77m);
        yield return new(17, "Figma", "Desktop", "Figma, Inc.", 144.00m);
        yield return new(18, "JetBrains Rider", "2024.3", "JetBrains", 199.00m);
        yield return new(19, "Docker Desktop", "4.15.1", "Docker, Inc.", 09m);
        yield return new(20, "PyCharm", "2023.2", "JetBrains", 199.00m);
    }
}

public record CcpOrder(string Id, IEnumerable<SoftwareLicence> Licences);

public record SoftwareLicence(Software Software, string LicenceKey, int LicenceCount, DateTimeOffset LicenceValidTo);