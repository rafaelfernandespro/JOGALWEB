using inter.Models;
using Microsoft.EntityFrameworkCore;

namespace inter.Data
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        modelBuilder.Entity<Produtos>()
            .Property(p => p.Preco)
            .HasPrecision(10, 2);
        }
        
        
        public DbSet<Pessoas> Pessoas { get; set; }
        public DbSet<Clientes> Clientes { get; set; }
        public DbSet<Produtos> Produtos { get; set; }
        public DbSet<Pedidos> Pedidos { get; set; }
        public DbSet<ItensPedido> ItensPedido { get; set; }
        public DbSet<Tipos> Tipos { get; set; }
        public DbSet<TipoPessoa> TipoPessoas { get; set; }
        public DbSet<Operadores> Operadores { get; set; }
    }
}