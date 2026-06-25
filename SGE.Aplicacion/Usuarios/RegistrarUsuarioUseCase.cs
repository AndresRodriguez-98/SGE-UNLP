using SGE.Aplicacion.Comun;
using SGE.Dominio.Usuarios;

namespace SGE.Aplicacion.Usuarios;

public class RegistrarUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;
    private readonly IUnidadDeTrabajo _unidadDeTrabajo;

    public RegistrarUsuarioUseCase(IUsuarioRepository usuarioRepo, IUnidadDeTrabajo unidadDeTrabajo)
    {
        _usuarioRepo = usuarioRepo;
        _unidadDeTrabajo = unidadDeTrabajo;
    }

    public RegistrarUsuarioResponse Ejecutar(RegistrarUsuarioRequest request)
    {
        // 1. Validar unicidad del correo electrónico
        var usuarioExistente = _usuarioRepo.ObtenerPorCorreo(request.CorreoElectronico);
        if (usuarioExistente != null)
        {
            // Podés usar una excepción de dominio o devolver una respuesta fallida según prefieras.
            // Para mantener consistencia con tus otros UseCases, tiramos excepción de negocio:
            throw new EntidadDuplicadaException("El correo electrónico ya se encuentra registrado.");
        }

        // 2. Crear la entidad usando el Factory Method del Dominio. 
        // El propio método 'Crear' se encarga de aplicar SHA-256 a la contraseña plana internamente.
        // Por defecto 'esAdministrador' es false en la firma del método de la entidad.
        var nuevoUsuario = Usuario.Crear(
            request.Nombre,
            request.CorreoElectronico,
            request.Contrasena
        );

        // 3. Persistencia atómica en memoria y confirmación en la BD
        _usuarioRepo.Agregar(nuevoUsuario);
        _unidadDeTrabajo.Guardar();

        return new RegistrarUsuarioResponse(true, nuevoUsuario.Id, "Usuario registrado con éxito.");
    }
}