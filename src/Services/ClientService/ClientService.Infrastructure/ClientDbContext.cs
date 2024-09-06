using ClientService.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientService.Infrastructure
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options)
            : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientProduct> ClientProducts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("client");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Use a default connection string if no other options are configured
                optionsBuilder.UseSqlServer("Server=localhost;Initial Catalog=DefaultDatabase;Integrated Security=True");
            }
        }
    }
}