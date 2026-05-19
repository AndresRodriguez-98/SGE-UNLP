namespace SGE.Aplicacion.Tramites;

using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Tramites;

public class ModificarTramiteUseCase
{
    private readonly ITramiteRepository _repoTramites;
    private readonly IActualizacionEstadoExpedienteService _actualizacionService;
    private readonly IAutorizacionService _autorizacionService;

    public ModificarTramiteUseCase(ITramiteRepository repoTramites, IActualizacionEstadoExpedienteService actualizacionService, IAutorizacionService autorizacionService)
    {
        _repoTramites = repoTramites;
        _actualizacionService = actualizacionService;
        _autorizacionService = autorizacionService;
    }

    public ModificarTramiteResponse Ejecutar(ModificarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar un tramite.");
        }

        var tramite = _repoTramites.ObtenerPorId(request.TramiteId);

        if (tramite == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el tramite con ID {request.TramiteId}");
        }

        var nuevoContenido = new ContenidoTramite(request.Contenido);
        tramite.ModificarTramite(request.Etiqueta, nuevoContenido, request.IdUsuario);
        _repoTramites.Modificar(tramite);
        _actualizacionService.Actualizar(tramite.ExpedienteId, request.IdUsuario);

        return new ModificarTramiteResponse(true);
    }
}