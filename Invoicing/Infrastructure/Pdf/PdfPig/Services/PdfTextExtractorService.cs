// Invoicing/Infrastructure/Pdf/PdfPig/Services/PdfTextExtractorService.cs
using UglyToad.PdfPig;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Pdf.PdfPig.Services;

public class PdfTextExtractorService : IPdfTextExtractorService
{
    public string ExtraerTexto(byte[] pdfBytes)
    {
        using var stream = new MemoryStream(pdfBytes);
        using var document = PdfDocument.Open(stream);

        var texto = new System.Text.StringBuilder();
        foreach (var page in document.GetPages())
            texto.AppendLine(page.Text);

        return texto.ToString();
    }
}