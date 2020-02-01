using GTANetworkAPI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace InfiniteRoleplay.Entities
{
    public class Armario
    {
        public int Codigo { get; set; }
        public double PosX { get; set; } = 0;
        public double PosY { get; set; } = 0;
        public double PosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public int Faccao { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public Marker Marker { get; set; }

        [NotMapped]
        public List<ArmarioItem> Itens { get => Global.ArmariosItens.Where(x => x.Codigo == Codigo).OrderBy(x => x.Rank).ThenBy(x => x.Arma).ToList(); }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = NAPI.TextLabel.CreateTextLabel("[Armário]", new Vector3(PosX, PosY, PosZ), 5, 2, 0, new Color(255, 255, 255), false, (uint)Dimensao);
            Marker = NAPI.Marker.CreateMarker(MarkerType.ThickChevronUp, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), 0.5f, new Color(255, 255, 255), false, (uint)Dimensao);
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            Marker?.Delete();
        }
    }
}