namespace SGE.Aplicacion.Comun;

public class RepositoryException : Exception
{
    public RepositoryException(string mensaje) : base(mensaje) { }
}

public class AutorizacionException : Exception
{
    public AutorizacionException(string mensaje) : base(mensaje) { }
}

public class EntidadNoEncontradaException : Exception
{
    public EntidadNoEncontradaException(string mensaje) : base(mensaje) { }
}