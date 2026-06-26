using SGE.Aplicacion.Expedientes;

public static class ExpedienteEndpoints
{
    public static void MapExpedienteEndpoints(this IEndpointRouteBuilder app)
    {
        var grupo = app.MapGroup("/api/expedientes")
            .WithTags("Expedientes")
            .RequireAuthorization();

        grupo.MapPost("/", (AgregarExpedienteUseCase useCase, AgregarExpedienteRequest req) =>
        {
            var resultado = useCase.Ejecutar(req);
            return Results.Ok(resultado);
        });
    }
}