// Invoicing/Application/Internal/OutboundServices/IAlmacenamientoService.cs
namespace FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service para Oracle Object Storage
/// </summary>
public interface IAlmacenamientoService
{
    Task<string> SubirImagenAsync(byte[] contenido, string contentType, CancellationToken cancellationToken);
    Task<string> SubirPdfAsync(byte[] contenido, string nombreArchivo, CancellationToken cancellationToken);
}