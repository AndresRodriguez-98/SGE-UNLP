using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Persistencia;

namespace SGE.Infraestructura.Expedientes;

public class ExpedienteRepository : IExpedienteRepository
{
    private readonly SgeContext _context;

    public ExpedienteRepository(SgeContext context)
    {
        _context = context;
    }

    public void Agregar(Expediente e) => _context.Expedientes.Add(e);

    public void Eliminar(Guid id)
    {
        var expediente = _context.Expedientes.Find(id);
        if (expediente != null)
        {
            _context.Expedientes.Remove(expediente);
        }
    }

    public void Modificar(Expediente e) => _context.Expedientes.Update(e);

    public Expediente? ObtenerPorId(Guid id) => _context.Expedientes.Find(id);

    public IEnumerable<Expediente> ObtenerTodos() => _context.Expedientes.ToList();

    // Cumple el contrato pero delegamos la unión de datos al Caso de Uso
    public Expediente? ObtenerConTramitesPorId(Guid id) => ObtenerPorId(id);
}