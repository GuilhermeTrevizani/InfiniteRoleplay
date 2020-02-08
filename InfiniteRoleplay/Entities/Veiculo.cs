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
        public int Cor1R { get; set; } = 0;
        public int Cor1G { get; set; } = 0;
        public int Cor1B { get; set; } = 0;
        public int Cor2R { get; set; } = 0;
        public int Cor2G { get; set; } = 0;
        public int Cor2B { get; set; } = 0;
        public float Vida { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public string Placa { get; set; } = string.Empty;

        [NotMapped]
        public Vehicle Vehicle { get; set; }

        [NotMapped]
        public bool Motor { get; set; } = false;

        public void Spawnar()
        {
            Vehicle = NAPI.Vehicle.CreateVehicle(NAPI.Util.VehicleNameToModel(Modelo), new Vector3(PosX, PosY, PosZ), new Vector3(RotX, RotY, RotZ), Cor1R, Cor2R);
            Vehicle.NumberPlate = Placa;
            Vehicle.EngineStatus = false;
            Vehicle.Locked = true;
            NAPI.Vehicle.SetVehicleCustomPrimaryColor(Vehicle, Cor1R, Cor1G, Cor1B);
            NAPI.Vehicle.SetVehicleCustomSecondaryColor(Vehicle, Cor2R, Cor2G, Cor2B);
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