using SGE.Dominio.Tramites;

public interface ITramiteRepository
{
    void Agregar(Tramite tramite);
    Tramite? ObtenerPorId(Guid tramiteId);
    IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId);
    void Modificar(Tramite tramite);
    void Eliminar(Guid tramiteId);
}