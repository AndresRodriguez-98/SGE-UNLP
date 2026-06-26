using System.Security.Claims;
using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Expedientes;

namespace SGE.WebApi.Endpoints;

public static class ExpedienteEndpoints
{
    public static void MapExpedienteEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/expedientes")
                       .WithTags("Expedientes")
                       .RequireAuthorization();

        // 1. POST: /api/expedientes
        grupo.MapPost("/", (AgregarExpedienteUseCase useCase, ClaimsPrincipal usuario, AgregarExpedienteRequest request) =>
        {
            var resultado = useCase.Ejecutar(request);
            return Results.Ok(resultado);
        });

        // 2. GET: /api/expedientes
        grupo.MapGet("/", (ListarExpedientesUseCase useCase) =>
        {
            var req = new ListarExpedientesRequest(); // atento aca, hay que hacerle new para que cree la lista vacia
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        // 3. GET por ID: /api/expedientes/{id}
        grupo.MapGet("/{id:guid}", (ObtenerExpedienteDetalladoUseCase useCase, ClaimsPrincipal usuario, Guid id) =>
        {
            // Extraemos el IdUsuario desde el Token JWT!!! IMPORTANTISIMO
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new ObtenerExpedienteDetalladoRequest(id, idUsuario);
            var resultado = useCase.Ejecutar(req);
            
            return resultado != null 
                ? Results.Ok(resultado) 
                : Results.NotFound(new { mensaje = "Expediente no encontrado." });
        });

        // 4. PUT: /api/expedientes/{id}/caratula
        grupo.MapPut("/{id:guid}/caratula", (ModificarCaratulaExpedienteUseCase useCase, ClaimsPrincipal usuario, Guid id, string nuevaCaratula) =>
        {
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            
            var req = new ModificarCaratulaRequest(id, nuevaCaratula, idUsuario);
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        // 5. PUT: /api/expedientes/{id}/estado
        grupo.MapPut("/{id:guid}/estado", (CambiarEstadoExpedienteUseCase useCase, ClaimsPrincipal usuario, Guid id, EstadoExpediente nuevoEstado) =>
        {
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            
            var req = new CambiarEstadoExpedienteRequest(id, nuevoEstado, idUsuario);
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        // 6. DELETE: /api/expedientes/{id}
        grupo.MapDelete("/{id:guid}", (EliminarExpedienteUseCase useCase, ClaimsPrincipal usuario, Guid id) =>
        {
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new EliminarExpedienteRequest(id, idUsuario);
            var resultado = useCase.Ejecutar(req);
            
            return Results.Ok(resultado); // Nos devuelve el EliminarExpedienteResponse(bool Exito)
        });
    }
}