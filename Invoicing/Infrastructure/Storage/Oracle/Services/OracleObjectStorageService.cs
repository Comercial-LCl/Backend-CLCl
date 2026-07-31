// Invoicing/Infrastructure/Storage/Oracle/Services/OracleObjectStorageService.cs — reemplaza la clase completa
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Infrastructure.Storage.Oracle.Configuration;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Storage.Oracle.Services;

public class OracleObjectStorageService(IOptions<OracleObjectStorageSettings> settings) : IAlmacenamientoService
{
    private readonly OracleObjectStorageSettings _settings = settings.Value;
    private IAmazonS3? _s3Client;

    private IAmazonS3 ObtenerCliente()
    {
        return _s3Client ??= new AmazonS3Client(
            new BasicAWSCredentials(_settings.AccessKey, _settings.SecretKey),
            new AmazonS3Config { ServiceURL = _settings.ServiceUrl, ForcePathStyle = true });
    }

    public async Task<string> SubirImagenAsync(byte[] contenido, string contentType, CancellationToken cancellationToken)
    {
        var extension = contentType == "image/png" ? "png" : "jpg";
        return await SubirAsync(contenido, contentType, $"facturas/imagenes/{Guid.NewGuid()}.{extension}", cancellationToken);
    }

    public async Task<string> SubirPdfAsync(byte[] contenido, string nombreArchivo, CancellationToken cancellationToken)
    {
        return await SubirAsync(contenido, "application/pdf", $"facturas/pdfs/{Guid.NewGuid()}-{nombreArchivo}", cancellationToken);
    }

    private async Task<string> SubirAsync(byte[] contenido, string contentType, string key, CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(contenido);
        await ObtenerCliente().PutObjectAsync(new PutObjectRequest
        {
            BucketName = _settings.BucketName,
            Key = key,
            InputStream = stream,
            ContentType = contentType
        }, cancellationToken);

        return $"{_settings.PublicBaseUrl.TrimEnd('/')}/{key}";
    }
}