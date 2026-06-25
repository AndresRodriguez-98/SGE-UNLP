using SGE.Aplicacion.Comun;
using SGE.Dominio.Autorizacion;

namespace SGE.Aplicacion.Usuarios;

public class ModificarPermisosUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ModificarPermisosUsuarioUseCase(IUsuarioRepository usuarioRepo, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepo = usuarioRepo;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public void Ejecutar(ModificarPermisosRequest request)
    {
        // 1. VALIDACIÓN DE ROL
        var ejecutor = _usuarioRepo.ObtenerPorId(request.AdministradorId);
        if (ejecutor == null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Se requieren permisos de Administrador.");
        }

        // 2. Buscar al usuario operativo a modificar
        var usuario = _usuarioRepo.ObtenerPorId(request.UsuarioIdAModificar) ?? throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.UsuarioIdAModificar}");

        // 3. Limpiar permisos actuales usando las reglas encapsuladas del Modelo Rico
        var permisosActuales = usuario.Permisos.ToList();
        foreach (var permiso in permisosActuales)
        {
            usuario.QuitarPermiso(permiso);
        }

        // 4. Mapear los strings que vienen de la API a Enums reales de Dominio e insertarlos
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