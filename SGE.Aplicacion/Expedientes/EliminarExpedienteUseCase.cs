using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Expedientes;

public class EliminarExpedienteUseCase
{
    private readonly IExpedienteRepository _repoExpedientes;
    private readonly ITramiteRepository _repoTramites;
    private readonly IAutorizacionService _autorizacionService;

    public EliminarExpedienteUseCase(IExpedienteRepository repoExpedientes, ITramiteRepository repoTramites, IAutorizacionService autorizacionService)
    {
        _repoExpedientes = repoExpedientes;
        _repoTramites = repoTramites;
        _autorizacionService = autorizacionService;
    }

    public EliminarExpedienteResponse Ejecutar(EliminarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteBaja))
        {
            throw new AutorizacionException("El usuario no tiene permisos para eliminar expedientes.");
        }

        var expediente = _repoExpedientes.ObtenerPorId(request.ExpedienteId);
        
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // Si el usuario tiene permisos para eliminar el expediente y el expediente existe, primero eliminamos todos los tramites asociados:
        var tramites = _repoTramites.ObtenerPorExpedienteId(request.ExpedienteId);

        foreach (var tramite in tramites)
        {
            _repoTramites.Eliminar(tramite.Id);
        }
        
        // Y despues el expediente:
        _repoExpedientes.Eliminar(request.ExpedienteId);
        
        return new EliminarExpedienteResponse(true);
    }
}