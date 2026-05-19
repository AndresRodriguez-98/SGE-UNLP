using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Aplicacion.Expedientes;
using SGE.Aplicacion.Tramites;
using SGE.Infraestructura.Expedientes;
using SGE.Infraestructura.Tramites;
using SGE.Aplicacion.Comun;

// Tener en cuenta comentar los ultimos casos de uso (eliminar expediente y eliminar tramite) para asi poder probar bien el programa y ver los resultados en los archivos .txt

Console.WriteLine("=== SISTEMA DE GESTIÓN DE EXPEDIENTES (SGE) ===");

try
{
    // 1. INYECCION DE DEPENDENCIAS
    var expedienteRepo = new ExpedienteTxtRepository();
    var tramiteRepo = new TramiteTxtRepository();
    var autorizacionService = new AutorizacionProvisionalService();
    var actualizacionEstadoService = new ActualizacionEstadoExpedienteService(expedienteRepo, tramiteRepo);

    // 2.1 CASOS DE USO DE EXPEDIENTES
    var agregarExpedienteUseCase = new AgregarExpedienteUseCase(expedienteRepo, autorizacionService);
    var listarExpedientesUseCase = new ListarExpedientesUseCase(expedienteRepo);
    var modificarCaratulaUseCase = new ModificarCaratulaExpedienteUseCase(expedienteRepo, autorizacionService);
    var cambiarEstadoUseCase = new CambiarEstadoExpedienteUseCase(expedienteRepo, autorizacionService);
    var eliminarExpedienteUseCase = new EliminarExpedienteUseCase(expedienteRepo, tramiteRepo, autorizacionService);

    // 2.2 CASOS DE USO DE TRAMITES
    var agregarTramiteUseCase = new AgregarTramiteUseCase(tramiteRepo, actualizacionEstadoService, autorizacionService);
    var listarTramitesUseCase = new ListarTramitesPorExpedienteUseCase(tramiteRepo);
    var modificarTramiteUseCase = new ModificarTramiteUseCase(tramiteRepo, actualizacionEstadoService, autorizacionService);
    var eliminarTramiteUseCase = new EliminarTramiteUseCase(tramiteRepo, actualizacionEstadoService, autorizacionService);

    // id ficticio para probar que pase la auth
    Guid idUsuario = Guid.NewGuid();

    // AGREGAR EXPEDIENTE
    Console.WriteLine("\n --- 1. Agregando Expediente ---");
    var requestAgregar = new AgregarExpedienteRequest("Expediente de Prueba", idUsuario);
    var responseAgregar = agregarExpedienteUseCase.Ejecutar(requestAgregar);
    Console.WriteLine("Expediente creado con éxito.");

    // LISTAR EXPEDIENTES
    Console.WriteLine("\n --- 2. Listando Expedientes ---");
    var requestListar = new ListarExpedientesRequest();
    var responseListar = listarExpedientesUseCase.Ejecutar(requestListar);

    Guid idParaModificar = Guid.Empty;

    foreach (var exp in responseListar.Expedientes)
    {
        Console.WriteLine($"- ID: {exp.Id} | Carátula: {exp.Caratula} | Estado: {exp.Estado}");
        idParaModificar = exp.Id;
    }

    if (idParaModificar != Guid.Empty)
    {
        // MODIFICAR CARÁTULA
        Console.WriteLine("\n--- 3. Modificando la carátula ---");
        var requestModificar = new ModificarCaratulaRequest(idParaModificar, "Carátula Modificada por Consola", idUsuario);
        modificarCaratulaUseCase.Ejecutar(requestModificar);
        Console.WriteLine("Carátula modificada correctamente.");

        // CAMBIAR ESTADO EXPEDIENTE
        Console.WriteLine("\n--- 4. Cambiando estado del expediente ---");
        var requestCambiarEstado = new CambiarEstadoExpedienteRequest(idParaModificar, (EstadoExpediente)1, idUsuario);
        cambiarEstadoUseCase.Ejecutar(requestCambiarEstado);
        Console.WriteLine("Estado cambiado correctamente.");

        // AGREGAR TRAMITE
        Console.WriteLine("\n--- 5. Agregando trámite ---");
        var requestAgregarTramite = new AgregarTramiteRequest(idParaModificar, EtiquetaTramite.PaseAEstudio, "Contenido de trámite de prueba", idUsuario);
        agregarTramiteUseCase.Ejecutar(requestAgregarTramite);
        Console.WriteLine("Trámite agregado correctamente.");

        // LISTAR TRAMITES
        Console.WriteLine("\n--- 6. Listando trámites del expediente ---");
        var requestListarTramites = new ListarTramitesPorExpedienteRequest(idParaModificar);
        var responseListarTramites = listarTramitesUseCase.Ejecutar(requestListarTramites);

        Guid idTramiteParaModificar = Guid.Empty;

        foreach (var tram in responseListarTramites.Tramites)
        {
            Console.WriteLine($"- ID: {tram.Id} | Etiqueta: {tram.Etiqueta} | Contenido: {tram.Contenido}");
            idTramiteParaModificar = tram.Id;
        }

        if (idTramiteParaModificar != Guid.Empty)
        {
            // MODIFICAR TRAMITE
            Console.WriteLine("\n--- 7. Modificando tramite ---");
            var requestModificarTramite = new ModificarTramiteRequest(idTramiteParaModificar, EtiquetaTramite.Despacho, "Contenido del tramite modificado!", idUsuario);
            modificarTramiteUseCase.Ejecutar(requestModificarTramite);
            Console.WriteLine("Tramite modificado correctamente!!");

            // ELIMINAR TRAMITE (COMENTAR PARA PROBAR BIEN EL PROGRAMA!!)
            Console.WriteLine("\n--- 8. Eliminando tramite ---");
            var requestEliminarTramite = new EliminarTramiteRequest(idTramiteParaModificar, idUsuario);
            eliminarTramiteUseCase.Ejecutar(requestEliminarTramite);
            Console.WriteLine("Tramite eliminado correctamente!!");
        }

        // ELIMINAR EXPEDIENTE (COMENTAR PARA PROBAR BIEN EL PROGRAMA!!)
        Console.WriteLine("\n--- 9. Eliminando expediente ---");
        var requestEliminarExpediente = new EliminarExpedienteRequest(idParaModificar, idUsuario);
        eliminarExpedienteUseCase.Ejecutar(requestEliminarExpediente);
        Console.WriteLine("Expediente eliminado correctamente!!");
    }

    Console.WriteLine("\n=====================================================");
    Console.WriteLine("Prueba finalizada.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"\n[ERROR CONTROLADO]: {ex.Message}");
    Console.ResetColor();
}