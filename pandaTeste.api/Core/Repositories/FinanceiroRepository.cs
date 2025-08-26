using Microsoft.EntityFrameworkCore;
using pandaTeste.api.Core.Interfaces;
using pandaTeste.api.Domain.Entities;
using pandaTeste.api.Domain.Models;
using pandaTeste.api.Infrastructure.Context;

namespace pandaTeste.api.Core.Repositories
{
    public class FinanceiroRepository : IFinanceiroRepository
    {
        private readonly PandaDbContext _context;

        public FinanceiroRepository(PandaDbContext context)
        {
            _context = context;
        }

        public async Task<Financeiro> ObterPorIdAsync(int id)
        {
            return await _context.Financeiros.FindAsync(id);
        }

        public async Task<IEnumerable<Financeiro>> ObterTodosAsync()
        {
            return await _context.Financeiros
                .OrderByDescending(f => f.DtVencimento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Financeiro>> ObterPorTipoAsync(string tipo)
        {
            return await _context.Financeiros
                .Where(f => f.TipoFinanceiro == tipo)
                .OrderByDescending(f => f.DtVencimento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Financeiro>> ObterPorStatusAsync(bool baixado)
        {
            return await _context.Financeiros
                .Where(f => f.Baixado == baixado)
                .OrderByDescending(f => f.DtVencimento)
                .ToListAsync();
        }

        public async Task<IEnumerable<Financeiro>> ObterVencimentosAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _context.Financeiros
                .Where(f => f.DtVencimento >= dataInicio && f.DtVencimento <= dataFim)
                .OrderBy(f => f.DtVencimento)
                .ToListAsync();
        }

        public async Task AdicionarAsync(Financeiro financeiro)
        {
            await _context.Financeiros.AddAsync(financeiro);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Financeiro financeiro)
        {
            _context.Financeiros.Update(financeiro);
            await _context.SaveChangesAsync();
        }

        public async Task RemoverAsync(int id)
        {
            var financeiro = await ObterPorIdAsync(id);
            if (financeiro != null)
            {
                _context.Financeiros.Remove(financeiro);
                await _context.SaveChangesAsync();
            }
        }
    }
}