// Invoicing/Infrastructure/Storage/Oracle/Configuration/OracleObjectStorageSettings.cs
namespace FacturasIA.Platform.Invoicing.Infrastructure.Storage.Oracle.Configuration;

/// <summary>
///     Configuración para el endpoint S3-compatible de Oracle Object Storage.
///     Se enlaza desde la sección "OracleObjectStorage" de appsettings.json.
/// </summary>
public class OracleObjectStorageSettings
{
    public required string ServiceUrl { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string BucketName { get; set; }
    public required string PublicBaseUrl { get; set; }
}