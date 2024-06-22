using System;

namespace loja.models
{
    public class Venda
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string NumeroNotaFiscal { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
    }
}
