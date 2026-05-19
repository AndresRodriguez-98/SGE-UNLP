namespace SGE.Infraestructura.Expedientes;

using SGE.Aplicacion.Comun;
using SGE.Dominio.Expedientes;
using System;
using System.Linq;

// usamos sealed para implementar el patron Singleton, buscando que solo se pueda instanciar una unica vez
public sealed class ExpedienteTxtRepository : IExpedienteRepository
{
    private readonly string _nombrearchivo = "expedientes.txt";

    private const string separador = "; ";

    public ExpedienteTxtRepository() { }

    // Serializamos:
    public string ConvertirAString(Expediente e)
    {
        return e.Id.ToString() + separador +
        e.Caratula.ToString() + separador +
        e.FechaCreacion.ToString() + separador +
        e.FechaUltimaModificacion.ToString() + separador +
        e.UsuarioUltimoCambio.ToString() + separador +
        e.Estado.ToString();
    }

    // Deserializamos:
    public Expediente ConvertirAObjeto(string expedienteTxt)
    {
        var partes = expedienteTxt.Split(separador);
        var id = Guid.Parse(partes[0]);
        var caratula = new Caratula(partes[1]);
        var fechaCreacion = DateTime.Parse(partes[2]);
        var fechaUltimaModificacion = DateTime.Parse(partes[3]);
        var usuarioUltimoCambio = Guid.Parse(partes[4]);
        var estado = Enum.Parse<EstadoExpediente>(partes[5]);
        Expediente miExpediente = Expediente.Reconstruir(id, caratula, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio, estado);
        return miExpediente;
    }

    public void Agregar(Expediente e)
    {
        var linea = ConvertirAString(e);
        File.AppendAllLines(_nombrearchivo, [linea]);
    }

    public void Eliminar(Guid id)
    {
        List<Expediente> miLista = (List<Expediente>)ObtenerTodos();
        if (!miLista.Remove(miLista.First(x => x.Id == id)))
        {
            throw new RepositoryException("El expediente a modificar no se encuentra en el repositorio.");
        }
        GuardarTodos(miLista);
    }

    public void Modificar(Expediente e)
    {
        List<Expediente> miLista = (List<Expediente>)ObtenerTodos();
        if (!miLista.Remove(miLista.First(x => x.Id == e.Id)))
        {
            throw new RepositoryException("El expediente a modificar no se encuentra en el repositorio.");
        }

        Expediente nuevoExp = Expediente.Reconstruir(e.Id, e.Caratula, e.FechaCreacion, e.FechaUltimaModificacion, e.UsuarioUltimoCambio, e.Estado);
        miLista.Add(nuevoExp);
        GuardarTodos(miLista);
    }

    public Expediente? ObtenerPorId(Guid id)
    {
        try
        {
            return ObtenerTodos().First(x => x.Id == id);
        }
        catch
        {
            return null;
        }
    }

    public IEnumerable<Expediente> ObtenerTodos()
    {
        List<Expediente> miLista = new List<Expediente>();
        var lineas = File.ReadAllLines(_nombrearchivo);
        foreach (var linea in lineas)
        {
            ConvertirAObjeto(linea);
            miLista.Add(ConvertirAObjeto(linea));
        }

        return miLista;
    }

    public void GuardarTodos(List<Expediente> expedientes)
    {
        var lineas = expedientes.Select(ConvertirAString);
        File.WriteAllLines(_nombrearchivo, lineas);
    }
}

