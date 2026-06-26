using SGE.WebApi.Dependencias;
using SGE.WebApi.Middlewares;
using SGE.WebApi.Endpoints;
using Microsoft.OpenApi;
using SGE.Infraestructura.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Inyecciones
builder.Services.AddInfraestructura(builder.Configuration);
builder.Services.AddAplicacion();
builder.Services.AddSeguridadJwt(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info = new OpenApiInfo
        {
            Title = "SGE API",
            Version = "v1",
            Description = "Sistema de Gestión de Expedientes con autenticación JWT."
        };

        // 1. Creamos el esquema del candado
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        
        document.Components.SecuritySchemes.Add("Bearer", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Meté tu token JWT directamente acá."
        });

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });

        return Task.CompletedTask;
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    try
    {
        var contexto = servicios.GetRequiredService<SgeContext>();
        // INICIALIZADOR del ensurecreated, pragma delete y seeds
        SgeDbInitializer.Inicializar(contexto); 
    }
    catch (Exception ex)
    {
        var logger = servicios.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Falló la creación o inicialización de la DB.");
    }
}

app.UseMiddleware<ExcepcionGlobalMiddleware>(); // las excepciones se ponen primero!!!

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SGE API v1");
        options.RoutePrefix = "swagger"; // Entrás desde /swagger
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapAutorizacionEndpoints();
app.MapExpedienteEndpoints();
app.MapTramiteEndpoints();
app.MapUsuariosEndpoints();

app.Run();