using System.Collections.Generic;
using System.Threading.Tasks;
using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;

namespace loja.services
{
    public class ServicoService
    {
        private readonly LojaDbContext _dbContext;

        public ServicoService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddServicoAsync(Servico servico)
        {
            _dbContext.Servicos.Add(servico);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateServicoAsync(Servico servico)
        {
            _dbContext.Entry(servico).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Servico> GetServicoByIdAsync(int id)
        {
            return await _dbContext.Servicos.FindAsync(id);
        }

        public async Task<List<Servico>> GetServicosByClienteIdAsync(int clienteId)
        {
            return await _dbContext.Contratos
                .Where(c => c.ClienteId == clienteId)
                .Include(c => c.Servico)
                .Select(c => c.Servico)
                .ToListAsync();
        }
    }
}
