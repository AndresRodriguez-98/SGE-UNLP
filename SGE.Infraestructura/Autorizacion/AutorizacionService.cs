using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Autorizacion;

namespace SGE.Infraestructura.Autorizacion;

public class AutorizacionService : IAutorizacionService
{
    private readonly IUsuarioRepository _usuarioRepo;

    public AutorizacionService(IUsuarioRepository usuarioRepo)
    {
        _usuarioRepo = usuarioRepo;
    }

    public bool PoseeElPermiso(Guid usuarioId, Permiso permisoRequerido)
    {
        var usuario = _usuarioRepo.ObtenerPorId(usuarioId);
        
        if (usuario == null) return false;

        if (usuario.EsAdministrador) return true;

        if (permisoRequerido == Permiso.TramiteBaja && usuario.Permisos.Contains(Permiso.ExpedienteBaja))
        {
            return true;
        }

        return usuario.Permisos.Contains(permisoRequerido);
    }
}