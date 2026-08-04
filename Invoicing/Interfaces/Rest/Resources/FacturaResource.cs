// Invoicing/Interfaces/Rest/Resources/FacturaResource.cs
namespace FacturasIA.Platform.Invoicing.Interfaces.Rest.Resources;

public record FacturaResource(
    Guid Id,
    Guid ProveedorId,
    Guid? CategoriaId,
    string Tipo,
    string Serie,
    string Numero,
    DateTime FechaEmision,
    decimal MontoTotal,
    string Moneda,
    string? ResumenIa,
    string? ArchivoUrl,
    string EstadoProcesamiento,
    IReadOnlyDictionary<string, string>? ConfianzaCampos,
    bool RequiereRevision,
    IEnumerable<ItemFacturaResource> Items);