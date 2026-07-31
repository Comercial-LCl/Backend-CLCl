// Invoicing/Application/CommandServices/IFacturaCommandService.cs
using FacturasIA.Platform.Invoicing.Domain.Model.Aggregates;
using FacturasIA.Platform.Invoicing.Domain.Model.Commands;
using FacturasIA.Platform.Shared.Application.Model;

namespace FacturasIA.Platform.Invoicing.Application.CommandServices;

public interface IFacturaCommandService
{
    Task<Result<Factura>> Handle(RegistrarFacturaFisicaCommand command, CancellationToken cancellationToken);
    Task<Result<Factura>> Handle(RegistrarFacturaElectronicaCommand command, CancellationToken cancellationToken);
    Task<Result<Factura>> Handle(CorregirDatoFacturaCommand command, CancellationToken cancellationToken);
}