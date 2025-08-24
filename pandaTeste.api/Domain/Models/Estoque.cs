namespace pandaTeste.api.Domain.Entities
{
    public class Estoque
    {
        public int Id { get; set; }
        public int QuantidadeEstoque { get; set; }
        public int EstoqueItemId { get; set; }
        public EstoqueItem EstoqueItem { get; set; }
    }
}
