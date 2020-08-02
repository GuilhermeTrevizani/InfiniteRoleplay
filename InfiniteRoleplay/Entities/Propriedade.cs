using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Propriedade
    {
        public int Codigo { get; set; }
        public TipoInterior Interior { get; set; }
        public int Valor { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public float EntradaPosX { get; set; } = 0;
        public float EntradaPosY { get; set; } = 0;
        public float EntradaPosZ { get; set; } = 0;
        public float SaidaPosX { get; set; } = 0;
        public float SaidaPosY { get; set; } = 0;
        public float SaidaPosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public TextLabel TextLabel2 { get; set; }

        [NotMapped]
        public bool IsAberta { get; set; } = false;

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = NAPI.TextLabel.CreateTextLabel($"Propriedade Nº {Codigo}", new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ), 5, 2, 0, new Color(254, 189, 12), false, (uint)Dimensao);

            if (Personagem == 0)
                TextLabel2 = NAPI.TextLabel.CreateTextLabel($"Use /pcomprar para comprar a propriedade por ${Valor:N0}", new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ - 0.1), 5, 1, 0, new Color(255, 255, 255), false, (uint)Dimensao);
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            TextLabel2?.Delete();
        }
    }
}