using System.Security.Claims;
using SGE.Aplicacion.Usuarios;

namespace SGE.WebApi.Endpoints;

public static class UsuariosEndpoints
{
    public static void MapUsuariosEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/usuarios")
                       .WithTags("Usuarios")
                       .RequireAuthorization();

        // PUT DEL USUARIO LOGEADO: /api/usuarios/mis-datos
        grupo.MapPut("/mis-datos", (ModificarMisDatosUseCase useCase, ClaimsPrincipal usuario, ModificarMisDatosInput input) =>
        {
            var idUsuarioToken = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new ModificarMisDatosRequest(idUsuarioToken, idUsuarioToken, input.NuevoNombre, input.NuevaContrasena);
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        //                      ---SECCION ADMIN ---

        // GET ALL
        grupo.MapGet("/", (ListarUsuariosUseCase useCase, ClaimsPrincipal usuario) =>
        {
            var idUsuarioToken = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());

            // Ejecutamos pasándole el ID del ejecutor para que el caso de uso valide el flag EsAdministrador
            var resultado = useCase.Ejecutar(idUsuarioToken);
            return Results.Ok(resultado);
        });

        // DELETE: /api/usuarios/{id}
        grupo.MapDelete("/{id:guid}", (EliminarUsuarioUseCase useCase, ClaimsPrincipal usuario, Guid id) =>
        {
            var idUsuarioToken = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new EliminarUsuarioRequest(id, idUsuarioToken);
            useCase.Ejecutar(req);

            return Results.Ok(new { exito = true, mensaje = "Usuario eliminado correctamente." });
        });

        // PUT: /api/usuarios/{id}/permisos
        grupo.MapPut("/{id:guid}/permisos", (ModificarPermisosUsuarioUseCase useCase, ClaimsPrincipal usuario, Guid id, List<string> nuevosPermisos) =>
        {
            var idUsuarioToken = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new ModificarPermisosRequest(id, idUsuarioToken, nuevosPermisos);
            useCase.Ejecutar(req);

            return Results.Ok(new { exito = true, mensaje = "Permisos actualizados correctamente." });
        });
    }
}

// DTO para la modif de los datos propios del user
public record ModificarMisDatosInput(string NuevoNombre, string NuevaContrasena);