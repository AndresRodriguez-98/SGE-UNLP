using SGE.Dominio.Autorizacion;
using System.Security.Cryptography;
using System.Text;

namespace SGE.Dominio.Usuarios;

public class Usuario
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public string CorreoElectronico { get; private set; } = string.Empty;
    public string ContrasenaHash { get; private set; } = string.Empty;
    public bool EsAdministrador { get; private set; }

    // ENCAPSULAMIENTO! por dentro es modificable, por fuera es de solo lectura
    private readonly List<Permiso> _permisos = new();
    public IReadOnlyCollection<Permiso> Permisos => _permisos.AsReadOnly();

    private Usuario() { }

    public static Usuario Crear(string nombre, string correoElectronico, string contrasenaPlana, bool esAdministrador = false)
    {
        if (string.IsNullOrWhiteSpace(nombre)) 
            throw new ArgumentException("El nombre no puede estar vacío.");
            
        if (string.IsNullOrWhiteSpace(correoElectronico)) 
            throw new ArgumentException("El correo electrónico no puede estar vacío.");
            
        if (string.IsNullOrWhiteSpace(contrasenaPlana)) 
            throw new ArgumentException("La contraseña no puede estar vacía.");

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Nombre = nombre,
            CorreoElectronico = correoElectronico,
            EsAdministrador = esAdministrador,
            ContrasenaHash = GenerarHash(contrasenaPlana)
        };

        return usuario;
    }

    // --- MÉTODOS DE SEGURIDAD CRIPTOGRÁFICA ---

    private static string GenerarHash(string textoPlano)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(textoPlano);
        var hash = sha256.ComputeHash(bytes);
        
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public bool ValidarContrasena(string contrasenaAValidar)
    {
        // Volvemos a hashear lo que ingresa el usuario y comparamos con lo guardado
        return ContrasenaHash == GenerarHash(contrasenaAValidar);
    }

    // --- MÉTODOS DE GESTIÓN DE PERMISOS ---

    public void AgregarPermiso(Permiso permiso)
    {
        if (!_permisos.Contains(permiso))
        {
            _permisos.Add(permiso);
        }
    }

    public void QuitarPermiso(Permiso permiso)
    {
        _permisos.Remove(permiso);
    }
    
    // Método extra útil para el caso de uso de "ModificarMisDatos"
    public void ActualizarDatosPersonales(string nuevoNombre, string nuevaContrasena)
    {
        if (!string.IsNullOrWhiteSpace(nuevoNombre)) Nombre = nuevoNombre;
        if (!string.IsNullOrWhiteSpace(nuevaContrasena)) ContrasenaHash = GenerarHash(nuevaContrasena);
    }
}