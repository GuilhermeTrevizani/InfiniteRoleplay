using GTANetworkAPI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace InfiniteRoleplay.Entities
{
    public class Armario
    {
        public int Codigo { get; set; }
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public int Faccao { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public TextLabel TextLabel2 { get; set; }

        [NotMapped]
        public List<ArmarioItem> Itens { get => Global.ArmariosItens.Where(x => x.Codigo == Codigo).OrderBy(x => x.Rank).ThenBy(x => x.Arma).ToList(); }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = NAPI.TextLabel.CreateTextLabel("Armário", new Vector3(PosX, PosY, PosZ), 5, 2, 0, new Color(254, 189, 12), false, (uint)Dimensao);
            TextLabel2 = NAPI.TextLabel.CreateTextLabel("Use /armario", new Vector3(PosX, PosY, PosZ - 0.1), 5, 1, 0, new Color(255, 255, 255), false, (uint)Dimensao);
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            TextLabel2?.Delete();
        }
    }
}