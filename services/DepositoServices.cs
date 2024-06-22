using System.Threading.Tasks;
using loja.data;
using loja.models;
using Microsoft.EntityFrameworkCore;

namespace loja.services
{
    public class DepositoService
    {
        private readonly LojaDbContext _dbContext;

        public DepositoService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddEstoqueAsync(int depositoId, int produtoId, int quantidade)
        {
            var deposito = await _dbContext.Depositos.FindAsync(depositoId);
            if (deposito == null)
            {
                throw new ArgumentException($"Deposito com ID {depositoId} não encontrado.");
            }

            var produto = await _dbContext.Produtos.FindAsync(produtoId);
            if (produto == null)
            {
                throw new ArgumentException($"Produto com ID {produtoId} não encontrado.");
            }

            var depositoProduto = await _dbContext.DepositoProdutos
                .FirstOrDefaultAsync(dp => dp.DepositoId == depositoId && dp.ProdutoId == produtoId);

            if (depositoProduto == null)
            {
                depositoProduto = new DepositoProduto
                {
                    DepositoId = depositoId,
                    ProdutoId = produtoId,
                    Quantidade = quantidade
                };
                _dbContext.DepositoProdutos.Add(depositoProduto);
            }
            else
            {
                depositoProduto.Quantidade += quantidade;
                _dbContext.DepositoProdutos.Update(depositoProduto);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
