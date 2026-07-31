// Invoicing/Domain/Model/Aggregates/Proveedor.cs
using FacturasIA.Platform.Invoicing.Domain.Model.ValueObjects;

namespace FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;

public class Proveedor
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Ruc Ruc { get; private set; } = null!;
    public string RazonSocial { get; private set; } = string.Empty;

    private Proveedor()
    {
    }

    public Proveedor(Ruc ruc, string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));

        Ruc = ruc;
        RazonSocial = razonSocial;
    }

    public Proveedor ActualizarRazonSocial(string razonSocial)
    {
        if (string.IsNullOrWhiteSpace(razonSocial))
            throw new ArgumentException("La razón social es obligatoria.", nameof(razonSocial));

        RazonSocial = razonSocial;
        return this;
    }
}