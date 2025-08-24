namespace pandaTeste.api.Application.Interfaces
{
    public interface IEstoqueService
    {
        void AdicionarAoEstoque(int itemId, string nomeItem, int quantidade);
        bool EntregarDoEstoque(int itemId, int quantidade);
    }
}
