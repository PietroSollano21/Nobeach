using Microsoft.EntityFrameworkCore;
using Nobeach.Models;
using Nobeach.Enums;

namespace Nobeach.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Agendamento> Agendamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Quadra> Quadras { get; set; }
    }
}
#pragma warning restore format