// Invoicing/Infrastructure/Ia/Gemini/Services/GeminiOcrIaService.cs
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Configuration;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Ia.Gemini.Services;

/// <summary>
///     Llama a la API REST de Gemini (generateContent) pidiéndole una respuesta en JSON estricto,
///     y la deserializa a los DTOs de resultado del puerto IOcrIaService.
/// </summary>
public class GeminiOcrIaService(HttpClient httpClient, IOptions<GeminiSettings> settings) : IOcrIaService
{
    private readonly GeminiSettings _settings = settings.Value;

    private record GeminiItemDto(string descripcion, decimal cantidad, decimal precioUnitario);

    private record GeminiFisicaDto(List<GeminiItemDto> items, string categoriaSugerida, string resumenIa);

    private record GeminiElectronicaDto(
        string proveedorRuc, string proveedorRazonSocial, string serie, string numero,
        string fechaEmision, decimal montoTotal, string moneda,
        List<GeminiItemDto> items, string categoriaSugerida, string resumenIa);

    public async Task<ResultadoOcrIaFisica> ProcesarFacturaFisicaAsync(
        byte[] imagenBytes, string contentType, CancellationToken cancellationToken)
    {
        const string prompt = """
            Eres un asistente que extrae información de fotos de facturas de compra peruanas.
            Analiza la imagen adjunta y responde EXCLUSIVAMENTE con un JSON (sin markdown, sin texto
            adicional) con esta forma exacta:
            {"items": [{"descripcion": string, "cantidad": number, "precioUnitario": number}],
             "categoriaSugerida": string, "resumenIa": string}
            La categoriaSugerida debe ser una categoría de gasto de negocio corta (ej. "Insumos de oficina",
            "Alimentos", "Servicios"). El resumenIa debe ser 1-2 oraciones resumiendo la compra.
            """;

        var dto = await LlamarGeminiAsync<GeminiFisicaDto>(prompt, imagenBytes, contentType, cancellationToken);

        return new ResultadoOcrIaFisica(
            dto.items.Select(i => new ItemExtraido(i.descripcion, i.cantidad, i.precioUnitario)).ToList(),
            dto.categoriaSugerida,
            dto.resumenIa);
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
                         "cantidad": number, "precioUnitario": number}], "categoriaSugerida": string,
                         "resumenIa": string}

                       Texto extraído del PDF:
                       {{textoExtraido}}
                       """;

        var dto = await LlamarGeminiAsync<GeminiElectronicaDto>(prompt, null, null, cancellationToken);

        return new ResultadoIaElectronica(
            dto.proveedorRuc,
            dto.proveedorRazonSocial,
            dto.serie,
            dto.numero,
            DateTime.Parse(dto.fechaEmision),
            dto.montoTotal,
            dto.moneda,
            dto.items.Select(i => new ItemExtraido(i.descripcion, i.cantidad, i.precioUnitario)).ToList(),
            dto.categoriaSugerida,
            dto.resumenIa);
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