using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using pandaTeste.api.Domain.Entities;
using pandaTeste.api.Domain.Models;
using System.Reflection.Emit;

namespace pandaTeste.api.Infrastructure.Mappings
{
    public class FinanceiroMap : IEntityTypeConfiguration<Financeiro>
    {
        public void Configure(EntityTypeBuilder<Financeiro> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Descricao)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Valor)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(e => e.TipoFinanceiro)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.DtVencimento)
                .IsRequired();

            builder.Property(e => e.Baixado)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.DtCadastro)
                .IsRequired(false);

            builder.Property(e => e.DtBaixa)
                .IsRequired(false);

            // Nome da tabela (opcional)
            builder.ToTable("Financeiros");
        }
    }
}
