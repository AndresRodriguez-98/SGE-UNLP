using Microsoft.EntityFrameworkCore;

using SGE.Dominio.Expedientes;
using SGE.Dominio.Tramites;
using SGE.Dominio.Usuarios;
using SGE.Dominio.Autorizacion;

namespace SGE.Infraestructura.Persistencia;

public class SgeContext : DbContext
{
    public DbSet<Expediente> Expedientes { get; set; }
    public DbSet<Tramite> Tramites { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }

    public SgeContext(DbContextOptions<SgeContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- MAPEO DE EXPEDIENTES ---
        modelBuilder.Entity<Expediente>(entity =>
        {
            entity.ToTable("Expedientes");
            entity.HasKey(e => e.Id);

            // REQUISITO: Mapear el Value Object Caratula
            entity.ComplexProperty(e => e.Caratula, c =>
            {
                c.Property(x => x.Valor)
                 .HasColumnName("Caratula")
                 .IsRequired();
            });
        });

        // --- MAPEO DE TRÁMITES ---
        modelBuilder.Entity<Tramite>(entity =>
        {
            entity.ToTable("Tramites");
            entity.HasKey(t => t.Id);

            // REQUISITO: Mapear el Value Object ContenidoTramite
            entity.ComplexProperty(t => t.Contenido, c =>
            {
                c.Property(x => x.Valor)
                 .HasColumnName("Contenido")
                 .IsRequired();
            });

            // Relación de uno a muchos (Un Expediente tiene muchos Trámites)
            entity.HasOne<Expediente>()
                  .WithMany()
                  .HasForeignKey(t => t.ExpedienteId)
                  .OnDelete(DeleteBehavior.Cascade); // Baja en cascada solicitada
        });

        // --- MAPEO DE USUARIOS ---
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuarios");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Nombre).IsRequired();
            entity.Property(u => u.CorreoElectronico).IsRequired();
            entity.Property(u => u.ContrasenaHash).IsRequired();

            // Mapeamos la colección interna de Enums (Permisos) como un string o JSON en SQLite
            entity.Property(u => u.Permisos)
                  .HasConversion(
                      p => string.Join(',', p),
                      p => p.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(Enum.Parse<Permiso>)
                            .ToList()
                  );
        });

        // --- DATOS SEMILLA (SEED) ---
        var admin = Usuario.Crear("Administrador", "admin@sge.com", "admin123", esAdministrador: true);
        var userConPermisos = Usuario.Crear("Facu", "facu@sge.com", "facu123");
        var userSinPermisos = Usuario.Crear("Andres", "andres@sge.com", "andres123");

        // Les asignamos permisos específicos a los de prueba si corresponde
        userConPermisos.AgregarPermiso(Permiso.ExpedienteAlta);
        userConPermisos.AgregarPermiso(Permiso.TramiteAlta);

        // Pasamos los datos iniciales a EF Core (Ojo: se mapean las propiedades privadas a través de objetos anónimos para el HasData)
        modelBuilder.Entity<Usuario>().HasData(
        [
            new { admin.Id, admin.Nombre, admin.CorreoElectronico, admin.ContrasenaHash, admin.EsAdministrador },
            new { userConPermisos.Id, userConPermisos.Nombre, userConPermisos.CorreoElectronico, userConPermisos.ContrasenaHash, userConPermisos.EsAdministrador },
            new { userSinPermisos.Id, userSinPermisos.Nombre, userSinPermisos.CorreoElectronico, userSinPermisos.ContrasenaHash, userSinPermisos.EsAdministrador }
        ]);
    }
}