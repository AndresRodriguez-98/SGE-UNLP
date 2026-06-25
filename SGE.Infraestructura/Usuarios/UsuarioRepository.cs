using SGE.Aplicacion.Usuarios;
using SGE.Dominio.Usuarios;
using SGE.Infraestructura.Persistencia;

namespace SGE.Infraestructura.Usuarios;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SgeContext _context;

    public UsuarioRepository(SgeContext context)
    {
        _context = context;
    }

    public void Agregar(Usuario usuario) => _context.Usuarios.Add(usuario);

    public void Eliminar(Usuario usuario) => _context.Usuarios.Remove(usuario);

    public Usuario? ObtenerPorId(Guid id) => _context.Usuarios.Find(id);

    public Usuario? ObtenerPorCorreo(string correo) => 
        _context.Usuarios.FirstOrDefault(u => u.CorreoElectronico == correo);

    public IEnumerable<Usuario> ObtenerTodos() => _context.Usuarios.ToList();
}