using GTANetworkAPI;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Vehicles : Script
    {
        [Command("vcomprar")]
        public void CMD_vcomprar(Client player) => Functions.ComprarVeiculo(player, string.Empty);

        [Command("motor")]
        public void CMD_motor(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo!");
                return;
            }

            player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!player.Vehicle.EngineStatus ? "des" : string.Empty)}ligou o motor do veículo!");
        }

        [Command("vtrancar")]
        public void CMD_vtrancar(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var veh = Global.Veiculos
                .Where(x => x.Personagem == p.Codigo && player.Position.DistanceTo(new Vector3(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= 5)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de um veículo seu!");
                return;
            }

            veh.Vehicle.Locked = !veh.Vehicle.Locked;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!veh.Vehicle.Locked ? "des" : string.Empty)}trancou o veículo!");
        }

        [Command("vcomprarvaga")]
        public void CMD_vcomprarvaga(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.Dinheiro < Global.Parametros.ValorVagaVeiculo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo!");
                return;
            }

            veh.PosX = player.Vehicle.Position.X;
            veh.PosY = player.Vehicle.Position.Y;
            veh.PosZ = player.Vehicle.Position.Z;
            veh.RotX = player.Vehicle.Rotation.X;
            veh.RotY = player.Vehicle.Rotation.Y;
            veh.RotZ = player.Vehicle.Rotation.Z;

            using (var context = new RoleplayContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            p.Dinheiro -= Global.Parametros.ValorVagaVeiculo;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou uma vaga por ${Global.Parametros.ValorVagaVeiculo.ToString("N0")}!");
        }

        [Command("vestacionar")]
        public void CMD_vestacionar(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo!");
                return;
            }

            var distance = player.Vehicle.Position.DistanceTo(new Vector3(veh.PosX, veh.PosY, veh.PosZ));
            if (distance > 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de sua vaga!");
                return;
            }

            veh.Vida = player.Vehicle.Health;

            using (var context = new RoleplayContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            veh.Despawnar();
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou seu veículo!");
        }

        [Command("vspawn")]
        public void CMD_vspawn(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
                if (veh?.Personagem != p.Codigo)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo!");
                    return;
                }

                veh.Spawnar();
                NAPI.ClientEvent.TriggerClientEvent(player, "setWaypoint", veh.PosX, veh.PosY);
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você spawnou seu veículo!");
            }
        }

        [Command("vlista")]
        public void CMD_vlista(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var veiculos = context.Veiculos.Where(x => x.Personagem == p.Codigo).ToList();
                if (veiculos.Count == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui veículos!");
                    return;
                }

                Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Veículos de {p.Nome} [{p.Codigo}]");
                foreach (var v in veiculos)
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Código: {v.Codigo} | Modelo: {v.Modelo} | Placa: {v.Placa} | Spawnado: {(Global.Veiculos.Any(x => x.Codigo == v.Codigo) ? "SIM" : "NÃO")}");
            }
        }
    }
}