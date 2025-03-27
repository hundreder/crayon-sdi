using Crayon.API.Models;

namespace Crayon.API.ApiClients;

public interface ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog();
}

public class CcpApiClient(HttpClient httpClient) : ICcpApiClient
{
    public IEnumerable<Software> GetEntireCatalog()
    {
        var softwareServices = new List<Software>
        {
            new("1", "Windows 11", "21H2", "Microsoft"),
            new("2", "Microsoft Office 365", "2023", "Microsoft"),
            new("3", "Adobe Premiere Pro", "23.1", "Adobe"),
            new("4", "VMware Workstation", "17 Pro", "VMware"),
            new("5", "Slack", "Desktop 4.29", "Slack Technologies"),
            new("6", "Zoom", "5.15", "Zoom Video"),
            new("7", "Visual Studio 2022", "17.6", "Microsoft"),
            new("8", "Notion", "Windows App", "Notion Labs"),
            new("9", "Postman", "10.12", "Postman, Inc."),
            new("10", "Microsoft Edge", "115.0.0", "Microsoft"),
            new("11", "Firefox", "118.0", "Mozilla"),
            new("12", "Chrome", "115.0", "Google"),
            new("13", "Unity", "2022.3.6", "Unity Technologies"),
            new("14", "MySQL Workbench", "8.0.34", "Oracle"),
            new("15", "GitHub Desktop", "3.3.4", "GitHub"),
            new("16", "Blender", "3.6.2", "Blender Foundation"),
            new("17", "Figma", "Desktop", "Figma, Inc."),
            new("18", "JetBrains Rider", "2024.3", "JetBrains"),
            new("19", "Docker Desktop", "4.15.1", "Docker, Inc."),
            new("20", "PyCharm", "2023.2", "JetBrains")
        };

        return softwareServices;
    }
}