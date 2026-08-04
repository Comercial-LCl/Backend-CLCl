// Invoicing/Infrastructure/Persistence/EntityFrameworkCore/Configuration/ConfianzaCamposConverter.cs
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using FacturasIA.Platform.Invoicing.Domain.Model;

namespace FacturasIA.Platform.Invoicing.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Serializa el diccionario de confianza por campo a JSON para guardarlo en una columna jsonb.
/// </summary>
public class ConfianzaCamposConverter() : ValueConverter<Dictionary<string, NivelConfianza>?, string?>(
    d => d == null ? null : JsonSerializer.Serialize(d, (JsonSerializerOptions?)null),
    s => string.IsNullOrEmpty(s) ? null : JsonSerializer.Deserialize<Dictionary<string, NivelConfianza>>(s, (JsonSerializerOptions?)null));