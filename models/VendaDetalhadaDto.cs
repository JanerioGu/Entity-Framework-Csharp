namespace loja.models
{
    public class VendaDetalhadaDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string ProdutoNome { get; set; }
        public string ClienteNome { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
    }

    public class VendaSumarizadaDto
    {
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int QuantidadeTotal { get; set; }
        public double PrecoTotal { get; set; }
    }

    public class ClienteVendaDetalhadaDto
    {
        public int Id { get; set; }
        public DateTime Data { get; set; }
        public string ProdutoNome { get; set; }
        public string ClienteNome { get; set; }
        public int Quantidade { get; set; }
        public double PrecoUnitario { get; set; }
    }

    public class ClienteVendaSumarizadaDto
    {
        public int ClienteId { get; set; }
        public string NomeCliente { get; set; }
        public int QuantidadeTotal { get; set; }
        public double PrecoTotal { get; set; }
    }
}
