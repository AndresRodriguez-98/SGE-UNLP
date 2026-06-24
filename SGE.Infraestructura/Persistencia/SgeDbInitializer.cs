using Microsoft.EntityFrameworkCore;

namespace SGE.Infraestructura.Persistencia;

public static class SgeDbInitializer
{
    public static void Inicializar(SgeContext context)
    {
        // REQUISITO: Inicializar usando EnsureCreated
        if (context.Database.EnsureCreated())
        {
            // REQUISITO: Establecer el journal_mode en DELETE mediante comandos nativos de conexión
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=DELETE;";
            command.ExecuteNonQuery();
        }
    }
}