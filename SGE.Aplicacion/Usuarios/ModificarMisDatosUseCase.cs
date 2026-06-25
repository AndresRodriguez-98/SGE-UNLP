using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ModificarMisDatosUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public ModificarMisDatosUseCase(IUsuarioRepository usuarioRepo, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepo = usuarioRepo;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public ModificarMisDatosResponse Ejecutar(ModificarMisDatosRequest request)
    {
        if (request.UsuarioIdAModificar != request.UsuarioIdEjecutor)
        {
            throw new AutorizacionException("Acceso denegado. No tiene permisos para modificar los datos de otro usuario.");
        }

        var usuario = _usuarioRepo.ObtenerPorId(request.UsuarioIdAModificar);
        if (usuario == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el usuario con ID {request.UsuarioIdAModificar}");
        }

        usuario.ActualizarDatosPersonales(request.NuevoNombre, request.NuevaContrasena);
        _usuarioRepo.Eliminar(usuario);
        _unidadDeTrabajo.Guardar();

        return new ModificarMisDatosResponse(true, "Datos personales actualizados con éxito.");
    }
}