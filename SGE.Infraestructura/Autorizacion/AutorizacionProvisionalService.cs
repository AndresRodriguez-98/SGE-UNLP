using SGE.Dominio.Autorizacion;

public class AutorizacionProvisionalService : IAutorizacionService
{
    public bool PoseeElPermiso(Guid idUsuario, Permiso permiso)
    {
        return true;
    }
}