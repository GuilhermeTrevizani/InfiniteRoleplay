using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Ponto
    {
        public int Codigo { get; set; }
        public int Tipo { get; set; } = 0;
        public double PosX { get; set; } = 0;
        public double PosY { get; set; } = 0;
        public double PosZ { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public Marker Marker { get; set; }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            string nome = string.Empty;
            switch((TipoPonto)Tipo)
            {
                case TipoPonto.Multas:
                    nome = "[Pagamento de Multas]";
                    break;
                case TipoPonto.Banco:
                    nome = "[Caixa Bancário]";
                    break;
                case TipoPonto.ATM:
                    nome = "[ATM]";
                    break;
                case TipoPonto.LojaConveniencia:
                    nome = "[Loja de Conveniência]";
                    break;
                case TipoPonto.LojaRoupas:
                    nome = "[Loja de Roupas]";
                    break;
            }

            TextLabel = NAPI.TextLabel.CreateTextLabel(nome, new Vector3(PosX, PosY, PosZ), 5, 2, 0, new Color(255, 255, 255));
            Marker = NAPI.Marker.CreateMarker(MarkerType.ThickChevronUp, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), 0.5f, new Color(255, 255, 255));
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            Marker?.Delete();
        }
    }
}