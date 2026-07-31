// Invoicing/Domain/Model/ValueObjects/NumeroFactura.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;

/// <summary>
///     Serie + número de una factura (ej. F001-000123)
/// </summary>
public sealed record NumeroFactura
{
    public string Serie { get; }
    public string Numero { get; }

    public NumeroFactura(string serie, string numero)
    {
        if (string.IsNullOrWhiteSpace(serie))
            throw new ArgumentException("La serie es obligatoria.", nameof(serie));
        if (string.IsNullOrWhiteSpace(numero))
            throw new ArgumentException("El número es obligatorio.", nameof(numero));

        Serie = serie.Trim().ToUpperInvariant();
        Numero = numero.Trim();
    }

    public override string ToString()
    {
        return $"{Serie}-{Numero}";
    }
}