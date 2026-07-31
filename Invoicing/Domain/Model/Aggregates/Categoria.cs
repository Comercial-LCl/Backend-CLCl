// Invoicing/Domain/Model/Aggregates/Categoria.cs
namespace FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;

public class Categoria
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = string.Empty;

    private Categoria()
    {
    }

    public Categoria(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));

        Nombre = nombre;
    }

    public Categoria Renombrar(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre de la categoría es obligatorio.", nameof(nombre));

        Nombre = nombre;
        return this;
    }
}