using System;

namespace loja.models
{
    public class Produto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public double Preco { get; set; }
        public string Fornecedor { get; set; }  // Se Fornecedor deve ser uma string, tudo bem. Caso contrário, pode ser necessário um tipo diferente aqui.

        public Produto()
        {
            Nome = string.Empty;      // Inicializando com uma string vazia
            Fornecedor = string.Empty; // Inicializando com uma string vazia
        }
    }
}
