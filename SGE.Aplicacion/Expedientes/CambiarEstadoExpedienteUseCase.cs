using SGE.Aplicacion.Autorizacion;
using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Expedientes;

public class CambiarEstadoExpedienteUseCase
{
    private readonly IExpedienteRepository _repository;
    private readonly IAutorizacionService _autorizacionService;
    
    public CambiarEstadoExpedienteUseCase(IExpedienteRepository repository, IAutorizacionService autorizacionService)
    {
        _repository = repository;
        _autorizacionService = autorizacionService;
    }

    public CambiarEstadoExpedienteResponse Ejecutar(CambiarEstadoExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteModificacion))
        {
            throw new AutorizacionException("El usuario no tiene permisos para modificar expedientes.");
        }

        var expediente = _repository.ObtenerPorId(request.ExpedienteId);
        
        if (expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {request.ExpedienteId}");
        }

        // de esta manera no se rompe el encapsulamiento!! esto seria Modelo Rico, el expediente sabe como mutar, yo no hago un expediente.Estado =
        expediente.CambiarEstado(request.NuevoEstado, request.IdUsuario);

        // hasta aca los cambios solo estaban en la RAM, ahora se guardan en el repositorio o en la DB a futuro:
        _repository.Modificar(expediente);

        return new CambiarEstadoExpedienteResponse(true);
    }
}