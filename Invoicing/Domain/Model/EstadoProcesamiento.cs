// Invoicing/Domain/Model/EstadoProcesamiento.cs — agrega el nuevo valor
namespace FacturasIA.Platform.Invoicing.Domain.Model;

public enum EstadoProcesamiento
{
    Pendiente,
    Procesado,
    ProcesadoSinDetalle,
    Error
}