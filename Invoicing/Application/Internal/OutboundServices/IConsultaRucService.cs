// Invoicing/Application/Internal/OutboundServices/IConsultaRucService.cs
namespace FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

public record DatosRucSunat(string Ruc, string RazonSocial);

/// <summary>
///     Outbound service para consultar los datos de un proveedor por RUC (SUNAT o un proxy
///     equivalente). Implementación concreta pendiente de decidir en el chat de stack/despliegue.
/// </summary>
public interface IConsultaRucService
{
    Task<DatosRucSunat?> ConsultarAsync(string ruc, CancellationToken cancellationToken);
}