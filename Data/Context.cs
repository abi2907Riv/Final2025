using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Final2025.Models.General;
public class Context : IdentityDbContext<ApplicationUser>
{
    public Context(DbContextOptions<Context> options)
        : base(options)
    {
    }

    public DbSet<Persona> Personas { get; set; }
    public DbSet<Actividad> Actividades { get; set; }
    public DbSet<TipoActividad> TipoActividades { get; set; }
}
