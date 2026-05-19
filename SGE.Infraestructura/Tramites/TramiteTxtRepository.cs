using SGE.Dominio.Tramites;
using SGE.Aplicacion.Comun;

namespace SGE.Infraestructura.Tramites;

public sealed class TramiteTxtRepository : ITramiteRepository
{
    private readonly string _nombrearchivo = "tramites.txt";
    private const string separador = "; ";

    public TramiteTxtRepository() { }

    public void Agregar(Tramite t)
    {
        var linea = ConvertirAString(t);
        File.AppendAllLines(_nombrearchivo, [linea]);
    }

    public void Eliminar(Guid tramiteId)
    {
        List<Tramite> miLista = ObtenerTodos().ToList();
        var tramite = miLista.FirstOrDefault(x => x.Id == tramiteId);

        if (tramite == null)
        {
            throw new RepositoryException("El trámite a eliminar no se encuentra en el repositorio.");
        }

        miLista.Remove(tramite);
        GuardarTodos(miLista);
    }

    public void Modificar(Tramite tramite)
    {
        List<Tramite> miLista = ObtenerTodos().ToList();

        var tramiteViejo = miLista.FirstOrDefault(x => x.Id == tramite.Id);

        if (tramiteViejo == null)
        {
            throw new RepositoryException("El trámite a modificar no se encuentra en el repositorio.");
        }

        miLista.Remove(tramiteViejo);
        miLista.Add(tramite);
        GuardarTodos(miLista);
    }

    public IEnumerable<Tramite> ObtenerPorExpedienteId(Guid expedienteId)
    {
        return ObtenerTodos().Where(x => x.ExpedienteId == expedienteId);
    }

    public Tramite? ObtenerPorId(Guid tramiteId)
    {
        return ObtenerTodos().FirstOrDefault(x => x.Id == tramiteId);
    }

    private IEnumerable<Tramite> ObtenerTodos()
    {
        List<Tramite> miLista = new List<Tramite>();

        if (!File.Exists(_nombrearchivo))
        {
            return miLista;
        }

        var lineas = File.ReadAllLines(_nombrearchivo);
        foreach (var linea in lineas)
        {
            if (string.IsNullOrWhiteSpace(linea)) continue;
            miLista.Add(ConvertirAObjeto(linea));
        }

        return miLista;
    }

    private void GuardarTodos(List<Tramite> tramites)
    {
        var lineas = tramites.Select(ConvertirAString);
        File.WriteAllLines(_nombrearchivo, lineas);
    }

    public static string ConvertirAString(Tramite t)
    {
        return t.Id.ToString() + separador +
               t.ExpedienteId.ToString() + separador +
               t.Etiqueta.ToString() + separador +
               t.Contenido.Valor + separador +
               t.FechaCreacion.ToString("O") + separador +
               t.FechaUltimaModificacion.ToString("O") + separador +
               t.UsuarioUltimoCambio.ToString();
    }

    public static Tramite ConvertirAObjeto(string tramiteTxt)
    {
        var partes = tramiteTxt.Split(separador);
        var id = Guid.Parse(partes[0]);
        var expedienteId = Guid.Parse(partes[1]);
        var etiqueta = Enum.Parse<EtiquetaTramite>(partes[2]);
        var contenido = new ContenidoTramite(partes[3]);
        var fechaCreacion = DateTime.Parse(partes[4]);
        var fechaUltimaModificacion = DateTime.Parse(partes[5]);
        var usuarioUltimoCambio = Guid.Parse(partes[6]);

        return Tramite.Reconstruir(id, expedienteId, etiqueta, contenido, fechaCreacion, fechaUltimaModificacion, usuarioUltimoCambio);
    }
}