using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Blip
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public double PosX { get; set; } = 0;
        public double PosY { get; set; } = 0;
        public double PosZ { get; set; } = 0;
        public int Tipo { get; set; } = 0;
        public int Cor { get; set; } = 0;

        [NotMapped]
        public GTANetworkAPI.Blip BlipGTA { get; set; }

        public void CriarIdentificador()
        {
            BlipGTA = NAPI.Blip.CreateBlip((uint)Tipo, new Vector3(PosX, PosY, PosZ), 1, (byte)Cor, Nome);
            BlipGTA.ShortRange = true;
        }

        public void DeletarIdentificador()
        {
            BlipGTA?.Delete();
        }
    }
}