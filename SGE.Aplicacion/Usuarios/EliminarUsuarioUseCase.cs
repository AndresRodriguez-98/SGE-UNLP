using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class EliminarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public EliminarUsuarioUseCase(IUsuarioRepository usuarioRepo, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepo = usuarioRepo;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public void Ejecutar(EliminarUsuarioRequest request)
    {
        var ejecutor = _usuarioRepo.ObtenerPorId(request.AdministradorId);
        if (ejecutor == null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Se requieren permisos de Administrador.");
        }

        if (request.UsuarioIdAEliminar == request.AdministradorId)
        {
            throw new ValidacionException("Operación inválida. Un administrador no puede eliminarse a sí mismo.");
        }

        var usuario = _usuarioRepo.ObtenerPorId(request.UsuarioIdAEliminar) ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.UsuarioIdAEliminar}");
        _usuarioRepo.Eliminar(usuario);
        _unidadDeTrabajo.Guardar();
    }
}