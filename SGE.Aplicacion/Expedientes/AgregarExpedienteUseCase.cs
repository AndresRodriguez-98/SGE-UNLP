namespace SGE.Aplicacion.Expedientes;

using SGE.Dominio.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;

public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository _repoExpediente;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;
    private readonly IAutorizacionService _autorizacionService;

    public AgregarExpedienteUseCase(IExpedienteRepository repoExpediente, IUnidadDeTrabajo unidadDeTrabajo, IAutorizacionService autorizacionService)
    {
        _repoExpediente = repoExpediente;
        _unidadDeTrabajo = unidadDeTrabajo;
        _autorizacionService = autorizacionService;
    }

    public AgregarExpedienteResponse Ejecutar(AgregarExpedienteRequest request)
    {
        if (!_autorizacionService.PoseeElPermiso(request.IdUsuario, Permiso.ExpedienteAlta))
        {
            throw new AutorizacionException("El usuario no tiene permisos para crear un nuevo expediente.");
        }

        var caratula = new Caratula(request.NuevaCaratula);
        var nuevoExpediente = new Expediente(caratula, request.IdUsuario);
        // la marcamos
        _repoExpediente.Agregar(nuevoExpediente);
        // confirmamos cambios
        _unidadDeTrabajo.Guardar();

        return new AgregarExpedienteResponse(true, nuevoExpediente.Id);
    }
}