// Invoicing/Infrastructure/Sunat/Configuration/DecolectaSettings.cs
namespace FacturasIA.Platform.Invoicing.Infrastructure.Sunat.Configuration;

public class DecolectaSettings
{
    public string BaseUrl { get; set; } = "https://api.decolecta.com/v1/";
    public string Token { get; set; } = string.Empty;
}