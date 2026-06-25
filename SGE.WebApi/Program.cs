using SGE.WebApi.Dependencias;
using SGE.WebApi.Middlewares;
using Scalar.AspNetCore;
using SGE.WebApi.Servicios;
using SGE.Aplicacion.Usuarios;
using Microsoft.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SGE.Infraestructura.Persistencia;

var builder = WebApplication.CreateBuilder(args);

// Inyecciones
builder.Services.AddInfraestructura(builder.Configuration);
builder.Services.AddAplicacion();
builder.Services.AddSeguridadJwt(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();

// Config de Scalar:
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SGE API", Version = "v1" });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var servicios = scope.ServiceProvider;
    try
    {
        var contexto = servicios.GetRequiredService<SgeContext>();
        contexto.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = servicios.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Falló la creación automática de la DB.");
    }
}

app.UseMiddleware<ExcepcionGlobalMiddleware>(); // las excepciones se ponen primero!!!

if (app.Environment.IsDevelopment())
{
    // 1. Genera el json base
    app.UseSwagger(options => { options.RouteTemplate = "openapi/{documentName}.json"; });

    // 2. Interfaz clásica de Swagger (Entrás en /swagger)
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/openapi/v1.json", "SGE API v1");
        c.RoutePrefix = "swagger";
    });

    // 3. Interfaz de Scalar corregida con el patrón relativo (Entrás en /scalar)
    app.MapScalarApiReference(options =>
    {
        options.WithOpenApiRoutePattern("/openapi/v1.json");
    });
}

app.UseAuthentication();
app.UseAuthorization();

// ENDPOINT 1: Registro de Usuarios (Público)
app.MapPost("/api/usuarios/registrar", (RegistrarUsuarioUseCase useCase, RegistrarUsuarioRequest request) =>
{
    var resultado = useCase.Ejecutar(request);
    return Results.Ok(resultado);
})
.WithTags("Autenticación");

// ENDPOINT 2: Login y Generación de Token (Público)
app.MapPost("/api/usuarios/login", (LoginUseCase useCase, TokenService tokenService, LoginRequest request) =>
{
    var response = useCase.Ejecutar(request);
    // Usamos el servicio de la API para firmar el token físico real
    var token = tokenService.GenerarToken(response);

    return Results.Ok(new { Token = token, Usuario = response });
})
.WithTags("Autenticación");

app.Run();