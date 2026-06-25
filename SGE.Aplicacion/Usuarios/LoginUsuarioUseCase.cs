using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class LoginUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;

    public LoginUseCase(IUsuarioRepository usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public LoginResponse Ejecutar(LoginRequest request)
    {
        var usuario = _usuarioRepo.ObtenerPorCorreo(request.CorreoElectronico);
        
        if (usuario == null || !usuario.ValidarContrasena(request.Contrasena))
        {
            throw new ValidacionException("Credenciales inválidas. Verifique correo y contraseña.");
        }

        return new LoginResponse(
            TokenValido: true,
            UsuarioId: usuario.Id,
            Nombre: usuario.Nombre,
            CorreoElectronico: usuario.CorreoElectronico,
            EsAdministrador: usuario.EsAdministrador,
            Permisos: usuario.Permisos.Select(p => p.ToString())
        );
    }
}