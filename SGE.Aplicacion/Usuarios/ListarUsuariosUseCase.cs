using SGE.Aplicacion.Comun;

namespace SGE.Aplicacion.Usuarios;

public class ListarUsuariosUseCase
{
    private readonly IUsuarioRepository _usuarioRepo;

    public ListarUsuariosUseCase(IUsuarioRepository usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public ListarUsuariosResponse Ejecutar(Guid administradorId)
    {
        var ejecutor = _usuarioRepo.ObtenerPorId(administradorId);
        if (ejecutor == null || !ejecutor.EsAdministrador)
        {
            throw new AutorizacionException("Acceso denegado. Se requieren permisos de Administrador.");
        }

        var usuarios = _usuarioRepo.ObtenerTodos();
        var listaDto = usuarios.Select(u => new UsuarioDTO(
            u.Id,
            u.Nombre,
            u.CorreoElectronico,
            u.EsAdministrador,
            u.Permisos.Select(p => p.ToString()).ToList()
        )).ToList();

        return new ListarUsuariosResponse(listaDto);
    }
}