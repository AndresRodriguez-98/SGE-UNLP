using System.Security.Claims;
using SGE.Aplicacion.Tramites;
using SGE.Dominio.Tramites;

namespace SGE.WebApi.Endpoints;

public static class TramiteEndpoints
{
    public static void MapTramiteEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/tramites")
                       .WithTags("Trámites")
                       .RequireAuthorization();

        grupo.MapPost("/", (AgregarTramiteUseCase useCase, ClaimsPrincipal usuario, AgregarTramiteRequest request) =>
        {
            var id = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = request with { IdUsuario = id };
            var resultado = useCase.Ejecutar(req);

            return Results.Ok(resultado);
        });

        grupo.MapGet("/expediente/{expedienteId:guid}", (ListarTramitesPorExpedienteUseCase useCase, Guid expedienteId) =>
        {
            var req = new ListarTramitesPorExpedienteRequest(expedienteId);
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        grupo.MapPut("/{id:guid}", (ModificarTramiteUseCase useCase, ClaimsPrincipal usuario, Guid id, ModificarTramiteInput input) =>
        {
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            var req = new ModificarTramiteRequest(id, input.Etiqueta, input.Contenido, idUsuario);
            
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });

        grupo.MapDelete("/{id:guid}", (EliminarTramiteUseCase useCase, ClaimsPrincipal usuario, Guid id) =>
        {
            var idUsuario = Guid.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString());
            
            var req = new EliminarTramiteRequest(id, idUsuario);
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });
    }
}

// TODO: ESTO ES PARA QUE EL PUT NO PIDA EL TRAMITE ID NI EL USUARIO ID
public record ModificarTramiteInput(EtiquetaTramite Etiqueta, string Contenido);