using Microsoft.EntityFrameworkCore;

namespace SGE.Infraestructura.Persistencia;

public static class SgeDbInitializer
{
    public static void Inicializar(SgeContext context)
    {
        if (context.Database.EnsureCreated())
        {
            var connection = context.Database.GetDbConnection();
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode=DELETE;";
            command.ExecuteNonQuery();
        }
    }
}