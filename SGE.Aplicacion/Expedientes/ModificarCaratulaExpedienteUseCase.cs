using SGE.Dominio.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

public class ModificarCaratulaExpedienteUseCase
{
    private readonly IExpedienteRepository _repository;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly IAutorizacionService _autorizacionService;

    public ModificarCaratulaExpedienteUseCase(IExpedienteRepository repository, IUnidadDeTrabajo unidadDeTrabajo, IAutorizacionService autorizacionService)
    {
        _repository = repository;
        _unidadDeTrabajo = unidadDeTrabajo;
        _autorizacionService = autorizacionService;
    }

    public ModificarCaratulaResponse Ejecutar(ModificarCaratulaRequest request)
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

        var nuevaCaratula = new Caratula(request.NuevaCaratula);
        
        expediente.ModificarCaratula(nuevaCaratula, request.IdUsuario);

        _repository.Modificar(expediente);

        _unidadDeTrabajo.Guardar();

        return new ModificarCaratulaResponse(true);
    }
}