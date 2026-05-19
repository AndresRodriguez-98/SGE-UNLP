namespace SGE.Aplicacion.Expedientes;

using SGE.Dominio.Expedientes;

public class ListarExpedientesUseCase
{
    private readonly IExpedienteRepository _repoExpedientes;

    public ListarExpedientesUseCase(IExpedienteRepository repoExpedientes)
    {
        _repoExpedientes = repoExpedientes;
    }

    public ListarExpedientesResponse Ejecutar(ListarExpedientesRequest request)
    {
        IEnumerable<Expediente> expedientes = _repoExpedientes.ObtenerTodos();
        List<ExpedienteDTO> expedientesDTO = new List<ExpedienteDTO>();
        
        foreach (var expediente in expedientes)
        {
           var dto = new ExpedienteDTO(
                expediente.Id, 
                expediente.Caratula.Valor,
                expediente.FechaCreacion, 
                expediente.FechaUltimaModificacion, 
                expediente.UsuarioUltimoCambio, 
                expediente.Estado
            );
            expedientesDTO.Add(dto);
        }
        
        return new ListarExpedientesResponse(expedientesDTO);
    }
}