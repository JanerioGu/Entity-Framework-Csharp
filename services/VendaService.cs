using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using loja.data;
using loja.models;

namespace loja.services
{
    public class VendaService
    {
        private readonly LojaDbContext _dbContext;

        public VendaService(LojaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Gravar uma venda
        public async Task AddVendaAsync(Venda venda)
        {
            var cliente = await _dbContext.Clientes.FindAsync(venda.ClienteId);
            if (cliente == null)
            {
                throw new ArgumentException($"Cliente com ID {venda.ClienteId} não encontrado.");
            }

            var produto = await _dbContext.Produtos.FindAsync(venda.ProdutoId);
            if (produto == null)
            {
                throw new ArgumentException($"Produto com ID {venda.ProdutoId} não encontrado.");
            }

            var depositoProduto = await _dbContext.DepositoProdutos
                .FirstOrDefaultAsync(dp => dp.ProdutoId == venda.ProdutoId && dp.DepositoId == 1);

            if (depositoProduto == null)
            {
                throw new InvalidOperationException($"Produto com ID {venda.ProdutoId} não encontrado no depósito.");
            }

            if (depositoProduto.Quantidade < venda.Quantidade)
            {
                throw new InvalidOperationException("Estoque insuficiente.");
            }

            depositoProduto.Quantidade -= venda.Quantidade;
            _dbContext.DepositoProdutos.Update(depositoProduto);

            venda.Data = DateTime.Now;
            _dbContext.Vendas.Add(venda);
            await _dbContext.SaveChangesAsync();
        }

        // Consultar vendas por produto (detalhada)
        public async Task<List<VendaDetalhadaDto>> GetVendasByProdutoIdAsync(int produtoId)
        {
            return await _dbContext.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Select(v => new VendaDetalhadaDto
                {
                    Id = v.Id,
                    Data = v.Data,
                    ProdutoNome = v.Produto.Nome,
                    ClienteNome = v.Cliente.Nome,
                    Quantidade = v.Quantidade,
                    PrecoUnitario = v.PrecoUnitario
                })
                .ToListAsync();
        }

        // Consultar vendas por produto (sumarizada)
        public async Task<VendaSumarizadaDto> GetVendasSumarizadasByProdutoIdAsync(int produtoId)
        {
            var result = await _dbContext.Vendas
                .Where(v => v.ProdutoId == produtoId)
                .GroupBy(v => v.ProdutoId)
                .Select(g => new VendaSumarizadaDto
                {
                    ProdutoId = g.Key,
                    NomeProduto = g.First().Produto.Nome,
                    QuantidadeTotal = g.Sum(v => v.Quantidade),
                    PrecoTotal = g.Sum(v => v.Quantidade * v.PrecoUnitario)
                })
                .FirstOrDefaultAsync();

            return result;
        }

        // Consultar vendas por cliente (detalhada)
        public async Task<List<ClienteVendaDetalhadaDto>> GetVendasByClienteIdAsync(int clienteId)
        {
            return await _dbContext.Vendas
                .Where(v => v.ClienteId == clienteId)
                .Include(v => v.Cliente)
                .Include(v => v.Produto)
                .Select(v => new ClienteVendaDetalhadaDto
                {
                    Id = v.Id,
                    Data = v.Data,
                    ProdutoNome = v.Produto.Nome,
                    ClienteNome = v.Cliente.Nome,
                    Quantidade = v.Quantidade,
                    PrecoUnitario = v.PrecoUnitario
                })
                .ToListAsync();
        }

        // Consultar vendas por cliente (sumarizada)
        public async Task<ClienteVendaSumarizadaDto> GetVendasSumarizadasByClienteIdAsync(int clienteId)
        {
            var result = await _dbContext.Vendas
                .Where(v => v.ClienteId == clienteId)
                .GroupBy(v => v.ClienteId)
                .Select(g => new ClienteVendaSumarizadaDto
                {
                    ClienteId = g.Key,
                    NomeCliente = g.First().Cliente.Nome,
                    QuantidadeTotal = g.Sum(v => v.Quantidade),
                    PrecoTotal = g.Sum(v => v.Quantidade * v.PrecoUnitario)
                })
                .FirstOrDefaultAsync();

            return result;
        }

        // Consultar produtos no depósito / estoque (sumarizada)
        public async Task<List<object>> GetProdutosNoDepositoAsync(int depositoId)
        {
            var result = await _dbContext.DepositoProdutos
                .Where(dp => dp.DepositoId == depositoId)
                .Select(dp => new
                {
                    ProdutoId = dp.ProdutoId,
                    NomeProduto = dp.Produto.Nome,
                    Quantidade = dp.Quantidade
                })
                .ToListAsync();

            return result.Cast<object>().ToList(); // Convertendo para List<object>
        }

        // Consultar a quantidade de um produto no depósito / estoque
        public async Task<object> GetQuantidadeProdutoNoDepositoAsync(int produtoId)
        {
            var result = await _dbContext.DepositoProdutos
                .Where(dp => dp.ProdutoId == produtoId)
                .Select(dp => new
                {
                    ProdutoId = dp.ProdutoId,
                    NomeProduto = dp.Produto.Nome,
                    Quantidade = dp.Quantidade
                })
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
