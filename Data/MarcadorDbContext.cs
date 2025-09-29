using Microsoft.EntityFrameworkCore;
using MarcadorFaseIIApi.Models;

namespace MarcadorFaseIIApi.Data;

public class MarcadorDbContext : DbContext
{
    public MarcadorDbContext(DbContextOptions<MarcadorDbContext> options)
            : base(options) { }

        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<MarcadorGlobal> Marcadores { get; set; }
        public DbSet<Falta> Faltas { get; set; }
        public DbSet<PartidoHistorico> PartidosHistoricos { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // En OnModelCreating
        modelBuilder.Entity<Equipo>(e =>
        {
        e.Property(x => x.Id).UseIdentityColumn();
        e.Property(x => x.Nombre)        // evitamos cambios de tipo/tamaño
        .IsRequired()
        .HasColumnType("nvarchar(max)");
        e.Property(x => x.Ciudad)        // <- ahora requerida
        .IsRequired()
        .HasMaxLength(80);
        e.Property(x => x.LogoFileName).HasMaxLength(128);
        });



            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoLocal)
                .WithMany()
                .HasForeignKey("EquipoLocalId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MarcadorGlobal>()
                .HasOne(m => m.EquipoVisitante)
                .WithMany()
                .HasForeignKey("EquipoVisitanteId")
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PartidoHistorico>()
                .Property(p => p.Id)
                .UseIdentityColumn();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey("RoleId")
                .OnDelete(DeleteBehavior.Restrict);

            //Indice de username
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            //Indices sobre FK
            modelBuilder.Entity<PartidoHistorico>()
                .HasIndex(p => p.EquipoLocalId);

            modelBuilder.Entity<PartidoHistorico>()
                .HasIndex(p => p.EquipoVisitanteId);

            modelBuilder.Entity<Jugador>(j =>
            {
            j.Property(x => x.Id).UseIdentityColumn();
            j.Property(x => x.Nombre).IsRequired().HasColumnType("nvarchar(max)");
            j.Property(x => x.Posicion).HasMaxLength(40); // opcionalmente 40–60
            j.HasOne(x => x.Equipo)
            .WithMany(e => e.Jugadores)
            .HasForeignKey(x => x.EquipoId)
            .OnDelete(DeleteBehavior.Cascade);
            });
        }
}
