// Invoicing/Domain/Model/ValueObjects/Ruc.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;

/// <summary>
///     RUC peruano: 11 dígitos numéricos.
/// </summary>
public sealed record Ruc
{
    public string Valor { get; }

    public Ruc(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor) || valor.Length != 11 || !valor.All(char.IsDigit))
            throw new ArgumentException("El RUC debe tener exactamente 11 dígitos numéricos.", nameof(valor));

        Valor = valor;
    }

    public override string ToString()
    {
        return Valor;
    }
}