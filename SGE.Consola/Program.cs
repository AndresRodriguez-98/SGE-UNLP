using SGE.Dominio.Expedientes;
using SGE.Infraestructura.Expedientes;

Console.WriteLine("=== Probando Sistema de Gestión de Expedientes (SGE) ===");

try 
{
    var miCaratula = new Caratula("Expediente de Prueba de Andrés");
    
    var miExpediente = new Expediente(miCaratula, Guid.NewGuid());

    Console.WriteLine($"Expediente creado con éxito!");
    Console.WriteLine($"ID: {miExpediente.Id}");
    Console.WriteLine($"Estado inicial: {miExpediente.Estado}"); // Debería ser RecienIniciado 

    var miRepositorio = new ExpedienteTxtRepository();

    miRepositorio.Agregar(miExpediente);

    var lista = miRepositorio.ObtenerTodos();

    foreach (var item in lista)
    {
        Console.WriteLine(item.Caratula.Valor);
    }

    Console.WriteLine(miRepositorio.ObtenerPorId(miExpediente.Id));

    // SECCION MODIFICAR
    Console.WriteLine("MODIFICACION DE UN EXPEDIENTE: ");
    miRepositorio.Modificar(miExpediente); // aca pisamos todo el .txt, entonces podriamos verlo modificado
    Console.WriteLine(miRepositorio.ObtenerPorId(miExpediente.Id)?.FechaUltimaModificacion);

    // SECCION ELIMINAR
    Console.WriteLine("ELIMINACIÓN DE UN EXPEDIENTE: ");
    miRepositorio.Eliminar(miExpediente.Id); // aca pisamos todo el .txt, entonces podriamos verlo modificado
}
catch (Exception ex) 
{
    Console.WriteLine($"Error: {ex.Message}");
}