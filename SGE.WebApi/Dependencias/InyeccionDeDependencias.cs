using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SGE.Aplicacion.Comun;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Aplicacion.Usuarios;
using SGE.Infraestructura.Autorizacion;
using SGE.Infraestructura.Expedientes;
using SGE.Infraestructura.Persistencia;
using SGE.Infraestructura.Tramites;
using SGE.Infraestructura.Usuarios;
using SGE.WebApi.Servicios;

namespace SGE.WebApi.Dependencias;

public static class DependencyInjection
{
    public static IServiceCollection AddInfraestructura(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configuración de SQLite con EF Core
        services.AddDbContext<SgeContext>(options =>
        {
            options.UseSqlite("Data Source=../SGE.Infraestructura/SGE.sqlite");
        });

        // 2. Registro de Unidad de Trabajo y Repositorios
        services.AddScoped<IUnidadDeTrabajo, UnidadDeTrabajo>();
        services.AddScoped<IExpedienteRepository, ExpedienteRepository>();
        services.AddScoped<ITramiteRepository, TramiteRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        // 3. Registro del Servicio de Autorización
        services.AddScoped<IAutorizacionService, AutorizacionService>();

        return services;
    }

    public static IServiceCollection AddAplicacion(this IServiceCollection services)
    {
        // 4. Registro de Casos de Uso de Expedientes
        services.AddScoped<AgregarExpedienteUseCase>();
        services.AddScoped<EliminarExpedienteUseCase>();
        services.AddScoped<ListarExpedientesUseCase>();
        services.AddScoped<ModificarCaratulaExpedienteUseCase>();
        services.AddScoped<CambiarEstadoExpedienteUseCase>();
        services.AddScoped<ObtenerExpedienteDetalladoUseCase>();
        services.AddScoped<IActualizacionEstadoExpedienteService, ActualizacionEstadoExpedienteService>();

        // 5. Registro de Casos de Uso de Trámites
        services.AddScoped<AgregarTramiteUseCase>();
        services.AddScoped<ModificarTramiteUseCase>();
        services.AddScoped<EliminarTramiteUseCase>();

        // 6. Registro de Casos de Uso de Usuarios
        services.AddScoped<RegistrarUsuarioUseCase>();
        services.AddScoped<LoginUseCase>();
        services.AddScoped<ModificarMisDatosUseCase>();
        services.AddScoped<ListarUsuariosUseCase>();
        services.AddScoped<EliminarUsuarioUseCase>();
        services.AddScoped<ModificarPermisosUsuarioUseCase>();

        return services;
    }

    public static IServiceCollection AddSeguridadJwt(this IServiceCollection services, IConfiguration configuration)
    {
        // 7. Registro del Servicio de Token
        services.AddScoped<TokenService>();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret no configurado.");
        var key = Encoding.UTF8.GetBytes(secret);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        services.AddAuthorization();

        return services;
    }
}