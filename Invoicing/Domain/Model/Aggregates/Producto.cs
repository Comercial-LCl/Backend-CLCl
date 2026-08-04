// Invoicing/Domain/Model/Aggregates/Producto.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;

/// <summary>
///     Producto es un sub-catálogo por proveedor (no global como Categoria) — el mismo nombre
///     de producto en dos proveedores distintos no es el mismo producto ni tiene precio comparable.
/// </summary>
public class Producto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProveedorId { get; private set; }
    public string Nombre { get; private set; } = string.Empty;

    private Producto()
    {
    }

    public Producto(Guid proveedorId, string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre del producto es obligatorio.", nameof(nombre));

        ProveedorId = proveedorId;
        Nombre = nombre;
    }
}