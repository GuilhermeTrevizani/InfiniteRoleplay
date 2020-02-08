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
        public TextLabel TextLabel2 { get; set; }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            string nome = string.Empty;
            switch((TipoPonto)Tipo)
            {
                case TipoPonto.Multas:
                    nome = "Pagamento de Multas";
                    break;
                case TipoPonto.Banco:
                    nome = "Caixa Bancário";
                    break;
                case TipoPonto.ATM:
                    nome = "ATM";
                    break;
                case TipoPonto.LojaConveniencia:
                    nome = "Loja de Conveniência";
                    break;
                case TipoPonto.LojaRoupas:
                    nome = "Loja de Roupas";
                    break;
            }

            string descricao = string.Empty;
            switch ((TipoPonto)Tipo)
            {
                case TipoPonto.Multas:
                    descricao = "Use /multas para checar suas multas pendentes";
                    break;
                case TipoPonto.Banco:
                    descricao = "Use /sacar, /transferir ou /depositar";
                    break;
                case TipoPonto.ATM:
                    descricao = "Use /sacar ou /transferir";
                    break;
                case TipoPonto.LojaConveniencia:
                    descricao = "Use /comprar";
                    break;
                case TipoPonto.LojaRoupas:
                    descricao = "Use /skin";
                    break;
            }

            TextLabel = NAPI.TextLabel.CreateTextLabel(nome, new Vector3(PosX, PosY, PosZ), 5, 2, 0, new Color(254, 189, 12));
            TextLabel2 = NAPI.TextLabel.CreateTextLabel(descricao, new Vector3(PosX, PosY, PosZ - 0.1), 5, 1, 0, new Color(255,255,255));
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            TextLabel2?.Delete();
        }
    }
}