using Microsoft.EntityFrameworkCore;
using loja.models;

namespace loja.data
{
    public class LojaDbContext : DbContext
    {
        public LojaDbContext(DbContextOptions<LojaDbContext> options) : base(options) { }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<DepositoProduto> DepositoProdutos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Contrato> Contratos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<DepositoProduto>()
                .HasKey(dp => new { dp.DepositoId, dp.ProdutoId });

            modelBuilder.Entity<DepositoProduto>()
                .HasOne(dp => dp.Deposito)
                .WithMany(d => d.DepositoProdutos)
                .HasForeignKey(dp => dp.DepositoId);

            modelBuilder.Entity<DepositoProduto>()
                .HasOne(dp => dp.Produto)
                .WithMany()
                .HasForeignKey(dp => dp.ProdutoId);

            // Adicionar mapeamento para Contrato
            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Cliente)
                .WithMany()
                .HasForeignKey(c => c.ClienteId);

            modelBuilder.Entity<Contrato>()
                .HasOne(c => c.Servico)
                .WithMany()
                .HasForeignKey(c => c.ServicoId);
        }
    }
}
