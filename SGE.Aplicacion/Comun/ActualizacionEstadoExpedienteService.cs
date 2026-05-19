namespace SGE.Aplicacion.Comun;

using SGE.Aplicacion.Expedientes;
using SGE.Dominio.Tramites;
using System;
using System.Linq;

public class ActualizacionEstadoExpedienteService : IActualizacionEstadoExpedienteService
{
    private readonly IExpedienteRepository _repoExpedientes;
    private readonly ITramiteRepository _repoTramites;

    public ActualizacionEstadoExpedienteService(IExpedienteRepository repoExpedientes, ITramiteRepository repoTramites)
    {
        _repoExpedientes = repoExpedientes;
        _repoTramites = repoTramites;
    }

    public void Actualizar(Guid ExpedienteId, Guid IdUsuario)
    {
        var expediente = _repoExpedientes.ObtenerPorId(ExpedienteId);
        if(expediente == null)
        {
            throw new EntidadNoEncontradaException($"No se encontró el expediente con ID {ExpedienteId}");
        }
        var tramites = _repoTramites.ObtenerPorExpedienteId(ExpedienteId);
        
        var ultimoTramite = tramites.OrderByDescending(t => t.FechaCreacion).FirstOrDefault();

        EtiquetaTramite? ultimaEtiqueta = ultimoTramite?.Etiqueta;
        
        bool cambio = expediente.ActualizarEstado(ultimaEtiqueta, IdUsuario);
        if(cambio)
        {
            _repoExpedientes.Modificar(expediente);
        }
    }
}