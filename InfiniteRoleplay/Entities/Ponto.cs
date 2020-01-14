using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Ponto
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
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

            TextLabel = NAPI.TextLabel.CreateTextLabel(Nome, new Vector3(PosX, PosY, PosZ), 5, 2, 0, new Color(255, 255, 255));
            Marker = NAPI.Marker.CreateMarker(MarkerType.ThickChevronUp, new Vector3(PosX, PosY, PosZ), new Vector3(), new Vector3(), 0.5f, new Color(255, 255, 255));
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            Marker?.Delete();
        }
    }
}