using SGE.WebApi.Dependencias;

var builder = WebApplication.CreateBuilder(args);

// Inyecciones
builder.Services.AddInfraestructura(builder.Configuration);
builder.Services.AddAplicacion();
builder.Services.AddSeguridadJwt(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();


// Config de Scalar:

var app = builder.Build();
