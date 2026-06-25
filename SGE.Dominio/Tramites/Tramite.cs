namespace SGE.Dominio.Tramites;

using SGE.Dominio.Comun;

public class Tramite
{
    public Guid Id { get; private set; }
    public Guid ExpedienteId { get; private set; }
    public EtiquetaTramite Etiqueta { get; private set; }
    public ContenidoTramite Contenido { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }

    public Tramite(Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, Guid usuarioUltimoCambio)
    {
        Id = Guid.NewGuid();
        ExpedienteId = expedienteId;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = DateTime.Now;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }

    private Tramite(Guid id, Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuarioUltimoCambio)
    {
        Id = id;
        ExpedienteId = expedienteId;
        Etiqueta = etiqueta;
        Contenido = contenido;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaModificacion;
        UsuarioUltimoCambio = usuarioUltimoCambio;
    }

    protected Tramite()
    {
        Contenido = null!;
    }

    public static Tramite Reconstruir(Guid id, Guid expedienteId, EtiquetaTramite etiqueta, ContenidoTramite contenido, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuarioUltimoCambio)
    {
        if (fechaModificacion < fechaCreacion)
            throw new DominioException("La fecha de modificación no puede ser menor a la de creación.");

        return new Tramite(id, expedienteId, etiqueta, contenido, fechaCreacion, fechaModificacion, usuarioUltimoCambio);
    }

    public void ModificarTramite(EtiquetaTramite nuevaEtiqueta, ContenidoTramite nuevoContenido, Guid idUsuario)
    {
        Etiqueta = nuevaEtiqueta;
        Contenido = nuevoContenido;
        UsuarioUltimoCambio = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }
}