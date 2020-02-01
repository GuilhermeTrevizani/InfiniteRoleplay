using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Propriedade
    {
        public int Codigo { get; set; }
        public int Interior { get; set; } = 0;
        public int Valor { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public double EntradaPosX { get; set; } = 0;
        public double EntradaPosY { get; set; } = 0;
        public double EntradaPosZ { get; set; } = 0;
        public double SaidaPosX { get; set; } = 0;
        public double SaidaPosY { get; set; } = 0;
        public double SaidaPosZ { get; set; } = 0;
        public long Dimensao { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public Marker Marker { get; set; }

        [NotMapped]
        public bool IsAberta { get; set; } = false;

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = NAPI.TextLabel.CreateTextLabel($"Nº {Codigo} {(Personagem == 0 ? $"(${Valor:N0})" : string.Empty)}", new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ), 5, 2, 0, new Color(255, 255, 255), false, (uint)Dimensao);

            var corMarker = Personagem > 0 ? new Color(255, 106, 77) : new Color(110, 180, 105);
            Marker = NAPI.Marker.CreateMarker(MarkerType.UpsideDownCone, new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ), new Vector3(), new Vector3(), 0.5f, corMarker, false, (uint)Dimensao);
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            Marker?.Delete();
        }
    }
}