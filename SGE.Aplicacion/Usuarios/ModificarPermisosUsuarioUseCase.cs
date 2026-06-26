using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase(IUsuarioRepository usuarioRepo, IUnidadDeTrabajo unidadDeTrabajo)
{
    private readonly IUsuarioRepository _usuarioRepo = usuarioRepo;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo = unidadDeTrabajo;

    public void Ejecutar(ModificarPermisosRequest request)
    {
        var ejecutor = _usuarioRepo.ObtenerPorId(request.AdministradorId);
        if (ejecutor == null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Se requieren permisos de Administrador.");
        }

        var usuario = _usuarioRepo.ObtenerPorId(request.UsuarioIdAModificar) ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.UsuarioIdAModificar}");

        var permisosActuales = usuario.Permisos.ToList();
        foreach (var permiso in permisosActuales)
        {
            usuario.QuitarPermiso(permiso);
        }

        foreach (var permisoStr in request.NuevosPermisosString)
        {
            if (Enum.TryParse<Permiso>(permisoStr, true, out var permisoEnum))
            {
                usuario.AgregarPermiso(permisoEnum);
            }
            else
            {
                throw new ValidacionException($"El permiso '{permisoStr}' no es un permiso válido en el sistema.");
            }
        }

        _unidadDeTrabajo.Guardar();
    }
}