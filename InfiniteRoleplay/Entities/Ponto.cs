using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Ponto
    {
        public int Codigo { get; set; }
        public int Tipo { get; set; } = 0;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;

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
                case TipoPonto.Concessionaria:
                    nome = "[/vcomprar]";
                    break;
                case TipoPonto.Multas:
                    nome = "[/vcomprar]";
                    break;
                case TipoPonto.Banco:
                    nome = "[Caixa Bancário]";
                    break;
                case TipoPonto.ATM:
                    nome = "[ATM]";
                    break;
                case TipoPonto.LojaConveniencia:
                    nome = "[/comprar]";
                    break;
                case TipoPonto.LojaRoupas:
                    nome = "[/skin]";
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