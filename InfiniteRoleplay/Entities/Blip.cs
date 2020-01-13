using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Blip
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public int Tipo { get; set; } = 0;
        public int Cor { get; set; } = 0;

        [NotMapped]
        public GTANetworkAPI.Blip BlipGTA { get; set; }

        public void CriarIdentificador()
        {
            BlipGTA = NAPI.Blip.CreateBlip(new Vector3(PosX, PosY, PosZ));
            BlipGTA.Sprite = (uint)Tipo;
            BlipGTA.Color = Cor;
            BlipGTA.Name = Nome;
        }

        public void DeletarIdentificador()
        {
            BlipGTA?.Delete();
        }
    }
}