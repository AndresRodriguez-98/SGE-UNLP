namespace SGE.Aplicacion.Tramites;

using SGE.Dominio.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Tramites;

public class AgregarTramiteUseCase
{
    private readonly ITramiteRepository _repoTramites;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly IActualizacionEstadoExpedienteService _actualizacionService;
    private readonly IAutorizacionService _autorizacionService;

    public AgregarTramiteUseCase(ITramiteRepository repoTramites, IUnidadDeTrabajo unidadDeTrabajo, IActualizacionEstadoExpedienteService actualizacionService, IAutorizacionService autorizacionService)
    {
        _repoTramites = repoTramites;
        _unidadDeTrabajo = unidadDeTrabajo;
        _actualizacionService = actualizacionService;
        _autorizacionService = autorizacionService;
    }

    public AgregarTramiteResponse Ejecutar(AgregarTramiteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.TramiteAlta))
        {
            throw new AutorizacionException("El usuario no tiene permisos para agregar un nuevo tramite.");
        }
        
        var nuevoTramite = new Tramite(request.ExpedienteId, request.Etiqueta, new ContenidoTramite(request.Contenido), request.IdUsuario);
        _repoTramites.Agregar(nuevoTramite);
        _actualizacionService.Actualizar(request.ExpedienteId, request.IdUsuario);
        _unidadDeTrabajo.Guardar();

        return new AgregarTramiteResponse(true);
    }
}