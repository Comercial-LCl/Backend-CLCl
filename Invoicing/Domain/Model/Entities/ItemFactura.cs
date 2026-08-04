// Invoicing/Domain/Model/Entities/ItemFactura.cs — reemplaza la clase completa
namespace FacturasIA.Platform.Invoicing.Domain.Model.Entities;

public class ItemFactura
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid FacturaId { get; private set; }
    public Guid ProductoId { get; private set; }
    public string Descripcion { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }

    private ItemFactura()
    {
        Descripcion = string.Empty;
    }

    public ItemFactura(Guid productoId, string descripcion, decimal cantidad, decimal precioUnitario)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            throw new ArgumentException("La descripción del ítem es obligatoria.", nameof(descripcion));
        if (cantidad <= 0)
            throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));
        if (precioUnitario < 0)
            throw new ArgumentException("El precio unitario no puede ser negativo.", nameof(precioUnitario));

        ProductoId = productoId;
        Descripcion = descripcion;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Subtotal = cantidad * precioUnitario;
    }

    internal void AsignarFactura(Guid facturaId)
    {
        FacturaId = facturaId;
    }
}