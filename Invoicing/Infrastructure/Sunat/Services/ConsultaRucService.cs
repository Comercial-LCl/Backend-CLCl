// Invoicing/Infrastructure/Sunat/Services/ConsultaRucService.cs
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using FacturasIA.Platform.Invoicing.Application.Internal.OutboundServices;
using FacturasIA.Platform.Invoicing.Infrastructure.Sunat.Configuration;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Sunat.Services;

/// <summary>
///     Implementación de <see cref="IConsultaRucService"/> contra el proxy REST de Decolecta
///     (https://api.decolecta.com/v1/sunat/ruc), que expone datos del padrón SUNAT.
/// </summary>
public class ConsultaRucService : IConsultaRucService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ConsultaRucService> _logger;

    public ConsultaRucService(
        HttpClient httpClient,
        IOptions<DecolectaSettings> settings,
        ILogger<ConsultaRucService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        _httpClient.BaseAddress = new Uri(settings.Value.BaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", settings.Value.Token);
        _httpClient.DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<DatosRucSunat?> ConsultarAsync(string ruc, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync($"sunat/ruc?numero={ruc}", cancellationToken);

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.NotFound)
            {
                // RUC inexistente o inválido según Decolecta.
                return null;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Decolecta respondió {StatusCode} al consultar RUC {Ruc}",
                    response.StatusCode, ruc);
                return null;
            }

            var body = await response.Content.ReadFromJsonAsync<DecolectaRucResponse>(
                cancellationToken: cancellationToken);

            if (body is null || string.IsNullOrWhiteSpace(body.RazonSocial))
                return null;

            return new DatosRucSunat(body.NumeroDocumento ?? ruc, body.RazonSocial);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            // Servicio caído / timeout: tratamos como "no encontrado" para no romper el flujo,
            // el frontend puede permitir ingreso manual como fallback.
            _logger.LogWarning(ex, "Fallo al consultar RUC {Ruc} en Decolecta", ruc);
            return null;
        }
    }

    private class DecolectaRucResponse
    {
        [JsonPropertyName("razon_social")]
        public string? RazonSocial { get; set; }

        [JsonPropertyName("numero_documento")]
        public string? NumeroDocumento { get; set; }

        [JsonPropertyName("estado")]
        public string? Estado { get; set; }

        [JsonPropertyName("condicion")]
        public string? Condicion { get; set; }
    }
}