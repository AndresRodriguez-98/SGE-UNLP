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

public record ModificarMisDatosRequest(
    Guid UsuarioIdAModificar, // ID que viaja en la peticion
    Guid UsuarioIdEjecutor, // ID real del JWT
    string NuevoNombre,
    string NuevaContrasena
);

public record ModificarMisDatosResponse(
    bool Exito,
    string Mensaje = ""
);

public record UsuarioDTO(Guid Id, string Nombre, string CorreoElectronico, bool EsAdministrador, List<string> Permisos);
public record ListarUsuariosResponse(List<UsuarioDTO> Usuarios);

public record EliminarUsuarioRequest(Guid UsuarioIdAEliminar, Guid AdministradorId);

public record ModificarPermisosRequest(
    Guid UsuarioIdAModificar,
    Guid AdministradorId,
    List<string> NuevosPermisosString
);