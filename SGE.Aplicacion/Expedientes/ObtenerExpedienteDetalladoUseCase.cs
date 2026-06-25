using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Expedientes;

public class ObtenerExpedienteDetalladoUseCase
{
    private readonly IExpedienteRepository _repoExpediente;
    private readonly ITramiteRepository _repoTramite;

    public ObtenerExpedienteDetalladoUseCase(IExpedienteRepository repoExpediente, ITramiteRepository repoTramite)
    {
        _repoExpediente = repoExpediente;
        _repoTramite = repoTramite;
    }

    public ExpedienteDetalladoDTO Ejecutar(ObtenerExpedienteDetalladoRequest request)
    {
        var expediente = _repoExpediente.ObtenerPorId(request.ExpedienteId) ?? throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        var tramites = _repoTramite.ObtenerPorExpedienteId(request.ExpedienteId);

        var tramitesDto = tramites.Select(t => new TramiteDetalleDTO(
            t.Id,
            t.Etiqueta.ToString(),
            t.Contenido.Valor,
            t.FechaCreacion,
            t.UsuarioUltimoCambio
        )).ToList();

        return new ExpedienteDetalladoDTO(
            expediente.Id,
            expediente.Caratula.Valor,
            expediente.FechaCreacion,
            expediente.Estado.ToString(),
            tramitesDto
        );
    }
}