namespace SGE.Dominio.Tramites;
using SGE.Dominio.Comun;

public enum EtiquetaTramite 
{ 
    EscritoPresentado, 
    PaseAEstudio, 
    Despacho, 
    Resolucion, 
    Notificacion, 
    PaseAlArchivo 
}

// RECORD: al ser record, es inmutable por defecto y tiene igualdad basada en valores(si hay 2 record con = contenido, se consideran iguales, aunque sean distintas instancias en memoria)
// Funciona como un objeto que envuelve al string, para tener garantia absoluta de que ese dato es válido y no está vacio.
public record class ContenidoTramite
{
    public string Valor { get; }
    
    public ContenidoTramite(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException("El contenido del trámite no puede estar vacío.");
        Valor = valor;
    }
}