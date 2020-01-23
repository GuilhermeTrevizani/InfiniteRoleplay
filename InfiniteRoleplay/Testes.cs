using GTANetworkAPI;
using Newtonsoft.Json;

namespace InfiniteRoleplay
{
    public class Testes : Script
    {
        [Command("w")]
        public void CMD_w(Client player, string arma, int municao)
        {
            var weaponHash = NAPI.Util.WeaponNameToModel(arma);
            if (weaponHash == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Arma {arma} não existe!");
                return;
            }

            player.GiveWeapon(weaponHash, municao);
        }

        [Command("p")]
        public void CMD_p(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Position.X.ToString().Replace(",", ".")}, {sender.Position.Y.ToString().Replace(",", ".")}, {sender.Position.Z.ToString().Replace(",", ".")}");
        }

        [Command("rot")]
        public void CMD_rot(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Rotation.X.ToString().Replace(",", ".")}, {sender.Rotation.Y.ToString().Replace(",", ".")}, {sender.Rotation.Z.ToString().Replace(",", ".")}");
        }

        [Command("pos")]
        public void CMD_pos(Client sender, float x, float y, float z)
        {
            sender.Position = new Vector3(x, y, z);
        }

        [Command("vp")]
        public void CMD_vp(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Vehicle.Position.X.ToString().Replace(",", ".")}, {sender.Vehicle.Position.Y.ToString().Replace(",", ".")}, {sender.Vehicle.Position.Z.ToString().Replace(",", ".")}");
        }

        [Command("vz")]
        public void CMD_vz(Client sender)
        {
            NAPI.Util.ConsoleOutput($"{sender.Vehicle.Rotation.X.ToString().Replace(",", ".")}, {sender.Vehicle.Rotation.Y.ToString().Replace(",", ".")}, {sender.Vehicle.Rotation.Z.ToString().Replace(",", ".")}");
        }

        [Command("teta")]
        public void CMD_rottata(Client player)
        {
            NAPI.ClientEvent.TriggerClientEvent(player, "teta");
        }

        [RemoteEvent("teta2")]
        public void CMD_rottateta2ta(Client player, object a)
        {
            NAPI.Util.ConsoleOutput(JsonConvert.SerializeObject(a));
        }
    }
}