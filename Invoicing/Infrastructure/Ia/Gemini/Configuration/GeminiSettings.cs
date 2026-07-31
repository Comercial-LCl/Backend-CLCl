// Invoicing/Infrastructure/Ia/Gemini/Configuration/GeminiSettings.cs
namespace FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Configuration;

public class GeminiSettings
{
    public required string ApiKey { get; set; }
    public string Model { get; set; } = "gemini-3.6-flash";
}