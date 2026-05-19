namespace SGE.Aplicacion.Tramites;

using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

public class EliminarTramiteUseCase
{
    private readonly ITramiteRepository _repoTramites;
    private readonly IActualizacionEstadoExpedienteService _actualizacionService;
    private readonly IAutorizacionService _autorizacionService;

    public EliminarTramiteUseCase(ITramiteRepository repoTramites, IActualizacionEstadoExpedienteService actualizacionService, IAutorizacionService autorizacionService)
    {
        _repoTramites = repoTramites;
        _actualizacionService = actualizacionService;
        _autorizacionService = autorizacionService;
    }

    public EliminarTramiteResponse Ejecutar(EliminarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteBaja))
        {
            throw new AutorizacionException("El usuario no tiene permisos para eliminar un tramite.");
        }

        var tramite = _repoTramites.ObtenerPorId(request.TramiteId);

        if (tramite == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el tramite con ID {request.TramiteId}");
        }

        _repoTramites.Eliminar(request.TramiteId);
        _actualizacionService.Actualizar(tramite.ExpedienteId, request.IdUsuario);

        return new EliminarTramiteResponse(true);
    }
}