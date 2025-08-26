using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Infrastructure.Mappings
{
    public class EstoqueConfiguration : IEntityTypeConfiguration<Estoque>
    {
        public void Configure(EntityTypeBuilder<Estoque> builder)
        {
            //builder.ToTable("Estoques");
            //builder.HasKey(e => e.Id);
            //builder.Property(e => e.QuantidadeEstoque).IsRequired();
            //builder.HasOne(e => e.EstoqueItem)
            //       .WithMany(ei => ei.Estoques)
            //       .HasForeignKey(e => e.EstoqueItemId);
        }
    }
}
