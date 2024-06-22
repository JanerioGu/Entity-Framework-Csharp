using System.Collections.Generic;

namespace loja.models
{
    public class Deposito
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public ICollection<DepositoProduto> DepositoProdutos { get; set; }

        public Deposito()
        {
            DepositoProdutos = new List<DepositoProduto>();
        }
    }

    public class DepositoProduto
    {
        public int DepositoId { get; set; }
        public Deposito Deposito { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
    }
}
