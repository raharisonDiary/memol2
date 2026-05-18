using Microsoft.EntityFrameworkCore;
using RecensementApi.Models;

namespace RecensementApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Ireto no hivadika ho Tables ao amin'ny MySQL
        public DbSet<Utilisateur> Utilisateurs { get; set; }
        public DbSet<Menage> Menages { get; set; }
        public DbSet<Citoyen> Citoyens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure ny fifandraisan'ny Menage sy Citoyen (One-to-Many)
            modelBuilder.Entity<Citoyen>()
                .HasOne(c => c.Menage)
                .WithMany(m => m.Membres)
                .HasForeignKey(c => c.MenageId)
                .OnDelete(DeleteBehavior.Cascade); // Raha fafana ny Menage, dia mamafa koa ny olona ao anatiny
        }
    }
}