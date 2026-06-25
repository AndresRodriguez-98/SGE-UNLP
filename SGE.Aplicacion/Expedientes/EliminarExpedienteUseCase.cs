using SGE.Dominio.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Expedientes;

public class EliminarExpedienteUseCase
{
    private readonly IExpedienteRepository _repoExpedientes;
    private readonly ITramiteRepository _repoTramites;
    private readonly IAutorizacionService _autorizacionService;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo; // <-- 1. DECLARAMOS LA UNIDAD DE TRABAJO

    public EliminarExpedienteUseCase(
        IExpedienteRepository repoExpedientes, 
        ITramiteRepository repoTramites, 
        IAutorizacionService autorizacionService,
        IUnidadDeTrabajo unidadDeTrabajo) // <-- 2. LA INYECTAMOS EN EL CONSTRUCTOR
    {
        _repoExpedientes = repoExpedientes;
        _repoTramites = repoTramites;
        _autorizacionService = autorizacionService;
        _unidadDeTrabajo = unidadDeTrabajo;
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

        // Recuperamos los trámites asociados desde la BD (memoria local de EF Core)
        var tramites = _repoTramites.ObtenerPorExpedienteId(request.ExpedienteId);

        // Marcamos cada trámite para su eliminación en el contexto
        foreach (var tramite in tramites)
        {
            _repoTramites.Eliminar(tramite.Id);
        }
        
        // Marcamos el expediente para su eliminación en el contexto
        _repoExpedientes.Eliminar(request.ExpedienteId);
        
        // 3. REGLA DE ORO: Guardamos de forma atómica en SQLite todas las bajas juntas
        _unidadDeTrabajo.Guardar();
        
        return new EliminarExpedienteResponse(true);
    }
}