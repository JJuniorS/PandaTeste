using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pandaTeste.api.Domain.Entities;

namespace pandaTeste.api.Infrastructure.Mappings
{
    public class EstoqueItemConfiguration : IEntityTypeConfiguration<EstoqueItem>
    {
        public void Configure(EntityTypeBuilder<EstoqueItem> builder)
        {
            builder.ToTable("EstoqueItens");
            builder.HasKey(ei => ei.Id);
            builder.Property(ei => ei.Nome).IsRequired().HasMaxLength(255);
            builder.HasMany(ei => ei.Estoques)
                   .WithOne(e => e.EstoqueItem)
                   .HasForeignKey(e => e.EstoqueItemId);
        }
    }
}
