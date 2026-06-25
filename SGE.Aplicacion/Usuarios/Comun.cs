namespace SGE.Aplicacion.Usuarios;

public record RegistrarUsuarioRequest(
    string Nombre,
    string CorreoElectronico,
    string Contrasena
);

public record RegistrarUsuarioResponse(
    bool Exito,
    Guid? UsuarioId,
    string Mensaje = ""
);

public record LoginRequest(
    string CorreoElectronico,
    string Contrasena
);

public record LoginResponse(
    bool TokenValido,
    Guid UsuarioId,
    string Nombre,
    string CorreoElectronico,
    bool EsAdministrador,
    IEnumerable<string> Permisos
);