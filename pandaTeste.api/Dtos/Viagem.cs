namespace pandaTeste.api.Dtos
{
    public class Viagem
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = "";
        public DateTime DtViagem { get; set; }
        public string Destino { get; set; } = "";
        public decimal Preco { get; set; } = 0.00m;
        public decimal Orcamento { get; set; } = 0.00m;
    }
}
