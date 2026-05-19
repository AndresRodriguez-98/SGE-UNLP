namespace SGE.Aplicacion.Tramites;

using SGE.Aplicacion.Comun;
using SGE.Dominio.Tramites;

public class ListarTramitesPorExpedienteUseCase
{
    private readonly ITramiteRepository _repoTramites;

    public ListarTramitesPorExpedienteUseCase(ITramiteRepository repoTramites)
    {
        _repoTramites = repoTramites;
    }

    public ListarTramitesPorExpedienteResponse Ejecutar(ListarTramitesPorExpedienteRequest request)
    {
        IEnumerable<Tramite> tramites = _repoTramites.ObtenerPorExpedienteId(request.ExpedienteId);

        if (tramites == null)
        {
            throw new EntidadNoEncontradaException($"No se encontraron tramites para el expediente con ID {request.ExpedienteId}");
        }

        List<TramiteDTO> tramitesDTO = new List<TramiteDTO>();

        foreach (var tramite in tramites)
        {
            var dto = new TramiteDTO(
                tramite.Id,
                tramite.ExpedienteId,
                tramite.Etiqueta,
                tramite.Contenido,
                tramite.FechaCreacion,
                tramite.FechaUltimaModificacion,
                tramite.UsuarioUltimoCambio
            );
            tramitesDTO.Add(dto);
        }

        return new ListarTramitesPorExpedienteResponse(tramitesDTO);
    }
}