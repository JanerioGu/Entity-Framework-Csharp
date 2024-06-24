using System.Collections.Generic;
using System.Threading.Tasks;
using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;

namespace loja.services
{
    public class ContratoService
    {
        private readonly LojaDbContext _dbContext;

        public ContratoService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddContratoAsync(Contrato contrato)
        {
            var cliente = await _dbContext.Clientes.FindAsync(contrato.ClienteId);
            if (cliente == null)
            {
                throw new ArgumentException($"Cliente com ID {contrato.ClienteId} não encontrado.");
            }

            var servico = await _dbContext.Servicos.FindAsync(contrato.ServicoId);
            if (servico == null)
            {
                throw new ArgumentException($"Servico com ID {contrato.ServicoId} não encontrado.");
            }

            contrato.DataContratacao = DateTime.Now;
            _dbContext.Contratos.Add(contrato);
            await _dbContext.SaveChangesAsync();
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
