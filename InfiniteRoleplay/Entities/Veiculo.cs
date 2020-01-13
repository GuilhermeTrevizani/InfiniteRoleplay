using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Veiculo
    {
        public int Codigo { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 0;
        public int Cor1 { get; set; } = 0;
        public int Cor2 { get; set; } = 0;
        public float Vida { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public string Placa { get; set; } = string.Empty;

        [NotMapped]
        public Vehicle Vehicle { get; set; }

        public void Spawnar()
        {
            Vehicle = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(Modelo), new Vector3(PosX, PosY, PosZ), new Vector3(RotX, RotY, RotZ), Cor1, Cor2);
            Vehicle.NumberPlate = Placa;
            Vehicle.EngineStatus = false;
            Vehicle.Locked = true;
            Global.Veiculos.Add(this);
        }

        public void Despawnar()
        {
            Vehicle?.Delete();
            Vehicle = null;
            Global.Veiculos.Remove(this);
        }
    }
}