using Microsoft.EntityFrameworkCore;
using pandaTeste.api.Domain.Entities;
using pandaTeste.api.Domain.Models;
using pandaTeste.api.Infrastructure.Mappings;

namespace pandaTeste.api.Infrastructure.Context
{
    public class PandaDbContext : DbContext
    {
        public PandaDbContext(DbContextOptions<PandaDbContext> options) : base(options)
        {
        }

        public DbSet<EstoqueItem> EstoqueItens { get; set; }
        public DbSet<Estoque> Estoques { get; set; }
        public DbSet<Financeiro> Financeiros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new FinanceiroMap());
            modelBuilder.ApplyConfiguration(new EstoqueItemConfiguration());
            modelBuilder.ApplyConfiguration(new EstoqueConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
