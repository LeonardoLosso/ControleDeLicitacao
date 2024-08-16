
namespace ControleDeLicitacao.App.DTOs.Baixa
{
    public class ItemDeBaixaDTO
    {
        public int ID { get; set; }

        public int BaixaID { get; set; }

        public string Nome { get; set; }

        public string Unidade { get; set; }

        public double QtdeEmpenhada { get; set; }

        public double QtdeLicitada { get; set; }

        public double QtdeAEmpenhar { get; set; }

        public double ValorEmpenhado { get; set; }

        public double ValorLicitado { get; set; }

        public double Saldo { get; set; }

        public double ValorUnitario { get; set; }
        public double Desconto { get; set; }
    }
}
