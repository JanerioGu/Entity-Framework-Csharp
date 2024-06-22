using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class ClienteService
    {
        private readonly LojaDbContext _dbContext;

        public ClienteService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Métodos para CRUD
        public async Task<List<Cliente>> GetAllClientesAsync()
        {
            return await _dbContext.Clientes.ToListAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _dbContext.Clientes.FindAsync(id);
        }

        public async Task AddClienteAsync(Cliente cliente)
        {
            _dbContext.Clientes.Add(cliente);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateClientesync(Cliente cliente)
        {
            _dbContext.Entry(cliente).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _dbContext.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _dbContext.Clientes.Remove(cliente);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}