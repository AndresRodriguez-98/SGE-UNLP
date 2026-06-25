namespace SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Comun;

public class Expediente
{
    public Guid Id { get; private set; }
    public Caratula Caratula { get; private set; }
    public DateTime FechaCreacion { get; private set; }
    public DateTime FechaUltimaModificacion { get; private set; }
    public Guid UsuarioUltimoCambio { get; private set; }
    public EstadoExpediente Estado { get; private set; }


    public Expediente(Caratula caratula, Guid usuarioId)
    {
        Id = Guid.NewGuid();
        Caratula = caratula;
        FechaCreacion = DateTime.Now;
        FechaUltimaModificacion = FechaCreacion;
        UsuarioUltimoCambio = usuarioId;
        Estado = EstadoExpediente.RecienIniciado;
    }

    // Este constructor privado es necesario para el factory method, para que no pase por las validaciones del constructor natural. Los default! son para que c# no joda
    private Expediente(Guid id, Caratula caratula, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuarioId, EstadoExpediente estado)
    {
        Id = id;
        Caratula = caratula;
        FechaCreacion = fechaCreacion;
        FechaUltimaModificacion = fechaModificacion;
        UsuarioUltimoCambio = usuarioId;
        Estado = estado;
    }

    protected Expediente()
    {
        Caratula = null!;
    }

    // factory method (permite trasabilidad de los expedientes. Un expediente no puede ser modificado asi nomas, por eso se usa el factory method, para que se reconstruya un nuevo expediente (tipo inmutable))
    public static Expediente Reconstruir(Guid id, Caratula caratula, DateTime fechaCreacion, DateTime fechaModificacion, Guid usuarioId, EstadoExpediente estado)
    {
        if (fechaModificacion < fechaCreacion)
            throw new DominioException("La fecha de modificación no puede ser menor a la de creación.");

        return new Expediente(id,caratula, fechaCreacion, fechaModificacion, usuarioId, estado);
    }

    public void ModificarCaratula(Caratula nuevaCaratula, Guid idUsuario)
    {
        Caratula = nuevaCaratula;
        UsuarioUltimoCambio = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }

    // Máquina de estados
    public bool ActualizarEstado(EtiquetaTramite? ultimaEtiqueta, Guid idUsuario)
    {
        EstadoExpediente nuevoEstado = Estado;

        if (ultimaEtiqueta == null)
        {
            nuevoEstado = EstadoExpediente.RecienIniciado;
        }
        else
        {
            switch (ultimaEtiqueta.Value)
            {
                case EtiquetaTramite.Resolucion:
                    nuevoEstado = EstadoExpediente.ConResolucion;
                    break;
                case EtiquetaTramite.PaseAEstudio:
                    nuevoEstado = EstadoExpediente.ParaResolver;
                    break;
                case EtiquetaTramite.PaseAlArchivo:
                    nuevoEstado = EstadoExpediente.Finalizado;
                    break;
            }
        }

        if (Estado != nuevoEstado)
        {
            CambiarEstado(nuevoEstado, idUsuario);
            return true;
        }

        return false;
    }

    public void CambiarEstado(EstadoExpediente nuevoEstado, Guid idUsuario)
    {
        Estado = nuevoEstado;
        UsuarioUltimoCambio = idUsuario;
        FechaUltimaModificacion = DateTime.Now;
    }
}