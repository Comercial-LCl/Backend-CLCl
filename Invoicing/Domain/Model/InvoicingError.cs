// Invoicing/Domain/Model/InvoicingError.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model;

public enum InvoicingError
{
    None,
    FacturaNotFound,
    NoAutorizado,
    ProcesamientoIaFallido,
    AlmacenamientoFallido,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}