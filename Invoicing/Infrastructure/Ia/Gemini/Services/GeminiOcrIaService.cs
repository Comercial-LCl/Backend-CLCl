// Invoicing/Infrastructure/Ia/Gemini/Services/GeminiOcrIaService.cs — reemplaza el archivo completo
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Domain.Model;
using FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Configuration;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Services;

public class GeminiOcrIaService(HttpClient httpClient, IOptions<GeminiSettings> settings) : IOcrIaService
{
    private readonly GeminiSettings _settings = settings.Value;

    private record GeminiItemDto(string descripcion, string nombreNormalizado, decimal cantidad, decimal precioUnitario);

    private record GeminiFisicaDto(
        List<GeminiItemDto> items, string categoriaSugerida, string resumenIa,
        string confianzaCategoria, bool itemsRequierenRevision);

    private record GeminiElectronicaDto(
        string proveedorRuc, string proveedorRazonSocial, string serie, string numero,
        string fechaEmision, decimal montoTotal, string moneda,
        List<GeminiItemDto> items, string categoriaSugerida, string resumenIa,
        Dictionary<string, string> confianzaCampos, bool itemsRequierenRevision);

    private static NivelConfianza ParsearConfianza(string valor)
    {
        return valor.Trim().ToLowerInvariant() switch
        {
            "alta" => NivelConfianza.Alta,
            "media" => NivelConfianza.Media,
            "baja" => NivelConfianza.Baja,
            _ => NivelConfianza.Media
        };
    }

    public async Task<ResultadoOcrIaFisica> ProcesarFacturaFisicaAsync(
        byte[] imagenBytes, string contentType, CancellationToken cancellationToken)
    {
        const string prompt = """
            Eres un asistente que extrae información de fotos de facturas de compra peruanas.
            Analiza la imagen adjunta y responde EXCLUSIVAMENTE con un JSON (sin markdown, sin texto
            adicional) con esta forma exacta:
            {"items": [{"descripcion": string, "nombreNormalizado": string, "cantidad": number, "precioUnitario": number}],
             "categoriaSugerida": string, "resumenIa": string,
             "confianzaCategoria": "alta"|"media"|"baja",
             "itemsRequierenRevision": boolean}
            La categoriaSugerida debe ser una categoría de gasto de negocio corta (ej. "Insumos de oficina",
            "Alimentos", "Servicios"). El nombreNormalizado debe ser una versión corta y estandarizada del
            producto en minúsculas, sin marcas ni tamaños específicos (ej. "papel bond a4"), para poder
            agrupar el mismo producto entre distintas facturas. confianzaCategoria es tu propio nivel de
            certeza sobre la categoría que elegiste. itemsRequierenRevision debe ser true si la foto salió
            borrosa, cortada, o si tuviste que adivinar algún número de la tabla de productos.
            """;

        var dto = await LlamarGeminiAsync<GeminiFisicaDto>(prompt, imagenBytes, contentType, cancellationToken);

        return new ResultadoOcrIaFisica(
            dto.items.Select(i => new ItemExtraido(i.descripcion, i.nombreNormalizado, i.cantidad, i.precioUnitario)).ToList(),
            dto.categoriaSugerida,
            dto.resumenIa,
            ParsearConfianza(dto.confianzaCategoria),
            dto.itemsRequierenRevision);
    }

    public async Task<ResultadoIaElectronica> ProcesarFacturaElectronicaAsync(
        string textoExtraido, CancellationToken cancellationToken)
    {
        var prompt = $$"""
            Eres un asistente que extrae información de facturas electrónicas peruanas a partir del
            texto plano extraído de su PDF. Responde EXCLUSIVAMENTE con un JSON (sin markdown, sin
            texto adicional) con esta forma exacta:
            {"proveedorRuc": string (11 dígitos), "proveedorRazonSocial": string, "serie": string,
              "numero": string, "fechaEmision": string (formato yyyy-MM-dd), "montoTotal": number,
              "moneda": string (código de 3 letras, ej. PEN), "items": [{"descripcion": string,
              "nombreNormalizado": string, "cantidad": number, "precioUnitario": number}],
              "categoriaSugerida": string, "resumenIa": string,
              "confianzaCampos": {"proveedorRuc": "alta"|"media"|"baja", "serie": "alta"|"media"|"baja",
              "numero": "alta"|"media"|"baja", "fechaEmision": "alta"|"media"|"baja",
              "montoTotal": "alta"|"media"|"baja", "categoria": "alta"|"media"|"baja"},
              "itemsRequierenRevision": boolean}
            El nombreNormalizado debe ser una versión corta y estandarizada del producto en minúsculas,
            sin marcas ni tamaños específicos, para poder agrupar el mismo producto entre distintas
            facturas. confianzaCampos es tu propio nivel de certeza en cada campo de cabecera que
            extrajiste del texto — usa "baja" si el texto salió cortado, ambiguo, o tuviste que inferir
            el valor en vez de leerlo directamente.

            Texto extraído del PDF:
            {{textoExtraido}}
            """;

        var dto = await LlamarGeminiAsync<GeminiElectronicaDto>(prompt, null, null, cancellationToken);

        var confianzaCampos = dto.confianzaCampos.ToDictionary(kv => kv.Key, kv => ParsearConfianza(kv.Value));

        return new ResultadoIaElectronica(
            dto.proveedorRuc,
            dto.proveedorRazonSocial,
            dto.serie,
            dto.numero,
            DateTime.Parse(dto.fechaEmision),
            dto.montoTotal,
            dto.moneda,
            dto.items.Select(i => new ItemExtraido(i.descripcion, i.nombreNormalizado, i.cantidad, i.precioUnitario)).ToList(),
            dto.categoriaSugerida,
            dto.resumenIa,
            confianzaCampos,
            dto.itemsRequierenRevision);
    }

    private async Task<T> LlamarGeminiAsync<T>(
        string prompt, byte[]? imagenBytes, string? contentType, CancellationToken cancellationToken)
    {
        var parts = new List<object> { new { text = prompt } };
        if (imagenBytes is not null)
            parts.Add(new
            {
                inline_data = new { mime_type = contentType, data = Convert.ToBase64String(imagenBytes) }
            });

        var requestBody = new
        {
            contents = new[] { new { parts } },
            generationConfig = new { response_mime_type = "application/json" }
        };

        var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent?key={_settings.ApiKey}";
        var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        using var document = JsonDocument.Parse(responseJson);
        var textoRespuesta = document.RootElement
            .GetProperty("candidates")[0]
            .GetProperty("content")
            .GetProperty("parts")[0]
            .GetProperty("text")
            .GetString()!;

        return JsonSerializer.Deserialize<T>(textoRespuesta, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }
}