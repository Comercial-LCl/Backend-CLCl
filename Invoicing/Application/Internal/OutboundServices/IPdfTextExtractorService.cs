// Invoicing/Application/Internal/OutboundServices/IPdfTextExtractorService.cs
namespace FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

/// <summary>
///     Outbound service para extraer el texto plano de un PDF (factura electrónica),
///     antes de pasárselo a Gemini para que interprete los datos.
/// </summary>
public interface IPdfTextExtractorService
{
    string ExtraerTexto(byte[] pdfBytes);
}