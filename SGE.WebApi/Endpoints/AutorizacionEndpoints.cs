
using SGE.Aplicacion.Usuarios;
using SGE.WebApi.Servicios;

public static class AutorizacionEndpoints
{
    public static void MapAutorizacionEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/usuarios").WithTags("Autenticacion");

        app.MapPost("/registrar", (RegistrarUsuarioUseCase useCase, RegistrarUsuarioRequest request) =>
        {
            var resultado = useCase.Ejecutar(request);
            return Results.Ok(resultado);
        });
        
        app.MapPost("/login", (LoginUseCase useCase, TokenService tokenService, LoginRequest request) =>
        {
            var response = useCase.Ejecutar(request);
            var token = tokenService.GenerarToken(response); // ACA FIRMAMOS EL TOKEN
            return Results.Ok(new { Token = token, Usuario = response });
        });
    }
}