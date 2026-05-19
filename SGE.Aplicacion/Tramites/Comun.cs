using SGE.Dominio.Tramites;

namespace SGE.Aplicacion.Tramites;

public record AgregarTramiteRequest(Guid ExpedienteId, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);
public record AgregarTramiteResponse(bool Exito);

public record EliminarTramiteRequest(Guid TramiteId, Guid IdUsuario);
public record EliminarTramiteResponse(bool Exito);

public record ModificarTramiteRequest(Guid TramiteId, EtiquetaTramite Etiqueta, string Contenido, Guid IdUsuario);
public record ModificarTramiteResponse(bool Exito);

public record ListarTramitesPorExpedienteRequest(Guid ExpedienteId);
public record ListarTramitesPorExpedienteResponse(IEnumerable<TramiteDTO> Tramites);
public record TramiteDTO(Guid Id, Guid ExpedienteId, EtiquetaTramite Etiqueta, ContenidoTramite Contenido, DateTime FechaCreacion, DateTime FechaModificacion, Guid UsuarioUltimoCambio);