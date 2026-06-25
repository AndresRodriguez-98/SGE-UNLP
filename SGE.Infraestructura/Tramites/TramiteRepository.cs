using SGE.Dominio.Tramites;
using SGE.Infraestructura.Persistencia;

namespace SGE.Infraestructura.Tramites;

public class TramiteRepository : ITramiteRepository
{
    private readonly SgeContext _context;

    public TramiteRepository(SgeContext context)
    {
        _context = context;
    }

    public void Agregar(Tramite t) => _context.Tramites.Add(t);

    public void Eliminar(Guid tramiteId)
    {
        var tramite = _context.Tramites.Find(tramiteId);
        if (tramite != null)
        {
            _context.Tramites.Remove(tramite);
        }
    }

    public void Modificar(Tramite tramite) => _context.Tramites.Update(tramite);

    public Tramite? ObtenerPorId(Guid tramiteId) => _context.Tramites.Find(tramiteId);

    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return _context.Tramites
                       .Where(t => t.ExpedienteId == expedienteId)
                       .ToList();
    }
}