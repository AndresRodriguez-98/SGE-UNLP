namespace SGE.Aplicacion.Comun;

public interface IActualizacionEstadoExpedienteService
{
    void Actualizar(Guid expedienteId, Guid idUsuario);
}