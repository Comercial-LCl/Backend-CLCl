// Invoicing/Domain/Model/ValueObjects/Monto.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;

/// <summary>
///     Monto monetario con su moneda (PEN, USD, etc.)
/// </summary>
public sealed record Monto
{
    public decimal Valor { get; }
    public string Moneda { get; }

    public Monto(decimal valor, string moneda)
    {
        if (valor <= 0)
            throw new ArgumentException("El monto debe ser mayor a cero.", nameof(valor));
        if (string.IsNullOrWhiteSpace(moneda) || moneda.Length != 3)
            throw new ArgumentException("La moneda debe ser un código de 3 letras (ej. PEN, USD).", nameof(moneda));

        Valor = valor;
        Moneda = moneda.ToUpperInvariant();
    }

    public override string ToString()
    {
        return $"{Moneda} {Valor:0.00}";
    }
}