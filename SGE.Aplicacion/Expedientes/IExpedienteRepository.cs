using SGE.Dominio.Expedientes;

// el repo solo se encarga de guardar el estado actual del objeto en nuestra infraestructura!!!
public interface IExpedienteRepository
{
    void Agregar(Expediente expediente); // Las mutaciones retornan void
    void Modificar(Expediente expediente);
    void Eliminar(Guid id);
    Expediente? ObtenerPorId(Guid id); // OJO aca, puede retornar null
    IEnumerable<Expediente> ObtenerTodos(); // Usamos IEnumerable por eficiencia de memoria
}