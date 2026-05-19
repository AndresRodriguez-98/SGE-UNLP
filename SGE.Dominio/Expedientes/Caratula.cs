namespace SGE.Dominio.Expedientes;
using SGE.Dominio.Comun;

public enum EstadoExpediente 
{ 
    RecienIniciado, 
    ParaResolver, 
    ConResolucion, 
    EnNotificacion, 
    Finalizado 
}

public record class Caratula
{
    public string Valor { get; }
    
    public Caratula(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DominioException("La carátula no puede estar vacía.");
        Valor = valor;
    }
}