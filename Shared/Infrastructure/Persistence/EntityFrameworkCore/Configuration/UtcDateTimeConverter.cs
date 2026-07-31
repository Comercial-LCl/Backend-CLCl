// Shared/Infrastructure/Persistence/EntityFrameworkCore/Configuration/UtcDateTimeConverter.cs
using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FacturasIA.Platform.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

/// <summary>
///     Todo DateTime que esta app escribe ya es un instante UTC real (DateTime.UtcNow del
///     servidor, o un string ISO del cliente con offset/'Z'). Este converter re-etiqueta las
///     lecturas como Utc para que las respuestas JSON incluyan el offset y los clientes puedan
///     convertir a hora local correctamente, en vez de reinterpretar un valor ya-UTC como local.
/// </summary>
public class UtcDateTimeConverter : ValueConverter<DateTime, DateTime>
{
    public UtcDateTimeConverter() : base(
        v => v,
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
    {
    }
}