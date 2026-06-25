using SGE.Dominio.Expedientes;

namespace SGE.Aplicacion.Expedientes;

// TODO: esta bien dejar todos tipos primitivos y solo el estadoExpediente para no parsearlo luego??
public record ModificarCaratulaRequest(Guid ExpedienteId, string NuevaCaratula, Guid IdUsuario);
public record ModificarCaratulaResponse(bool Exito);
public record CambiarEstadoExpedienteRequest(Guid ExpedienteId, EstadoExpediente NuevoEstado, Guid IdUsuario);
public record CambiarEstadoExpedienteResponse(bool Exito);
public record EliminarExpedienteRequest(Guid ExpedienteId, Guid IdUsuario);
public record EliminarExpedienteResponse(bool Exito);
public record AgregarExpedienteRequest(string NuevaCaratula, Guid IdUsuario);
public record AgregarExpedienteResponse(bool Exito, Guid ExpedienteId);
public record ListarExpedientesRequest();
public record ListarExpedientesResponse(IEnumerable<ExpedienteDTO> Expedientes);
public record ExpedienteDTO(Guid Id, string Caratula, DateTime FechaCreacion, DateTime FechaUltimaModificacion, Guid UsuarioUltimoCambio, EstadoExpediente Estado);
public record ObtenerExpedienteDetalladoRequest(
    Guid ExpedienteId,
    Guid IdUsuario // Lo necesitamos si quisiéramos meterle validación de permisos de lectura a futuro
);
public record TramiteDetalleDTO(
    Guid Id,
    string Etiqueta,
    string Contenido,
    DateTime FechaCreacion,
    Guid UsuarioId
);

public record ExpedienteDetalladoDTO(
    Guid Id,
    string Caratula,
    DateTime FechaCreacion,
    string Estado,
    List<TramiteDetalleDTO> Tramites
);