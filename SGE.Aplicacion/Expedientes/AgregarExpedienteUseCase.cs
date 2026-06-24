namespace SGE.Aplicacion.Expedientes;

using SGE.Dominio.Autorizacion;
using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;

public class AgregarExpedienteUseCase
{
    private readonly IExpedienteRepository _repoExpediente;
    private readonly IAutorizacionService _autorizacionService;

    public AgregarExpedienteUseCase(IExpedienteRepository repoExpediente, IAutorizacionService autorizacionService)
    {
        _repoExpediente = repoExpediente;
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
        _repoExpediente.Agregar(nuevoExpediente);

        return new AgregarExpedienteResponse(true, nuevoExpediente.Id);
    }
}