using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Vehicles : Script
    {
        [Command("vcomprar", "!{#febd0c}USO:~w~ /vcomprar")]
        public void CMD_vcomprar(Player player) => Functions.ComprarVeiculo(player, 0, string.Empty);

        [Command("motor", "!{#febd0c}USO:~w~ /motor")]
        public void CMD_motor(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!player.IsInVehicle || player.VehicleSeat != (int)VehicleSeat.Driver)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o motorista de um veículo!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo!");
                return;
            }

            player.Vehicle.EngineStatus = !player.Vehicle.EngineStatus;
            veh.Motor = player.Vehicle.EngineStatus;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!player.Vehicle.EngineStatus ? "des" : string.Empty)}ligou o motor do veículo!");
        }

        [Command("vtrancar", "!{#febd0c}USO:~w~ /vtrancar")]
        public void CMD_vtrancar(Player player)
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

        [Command("vcomprarvaga", "!{#febd0c}USO:~w~ /vcomprarvaga")]
        public void CMD_vcomprarvaga(Player player)
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

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou uma vaga por ${Global.Parametros.ValorVagaVeiculo:N0}!");
        }

        [Command("vestacionar", "!{#febd0c}USO:~w~ /vestacionar")]
        public void CMD_vestacionar(Player player)
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

            if (player.Vehicle.Position.DistanceTo(new Vector3(veh.PosX, veh.PosY, veh.PosZ)) > 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de sua vaga!");
                return;
            }

            veh.Vida = NAPI.Vehicle.GetVehicleBodyHealth(player.Vehicle);

            using (var context = new RoleplayContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            veh.Despawnar();
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou seu veículo!");
        }

        [Command("vspawn", "!{#febd0c}USO:~w~ /vspawn (código do veículo)")]
        public void CMD_vspawn(Player player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (Global.Veiculos.Any(x => x.Codigo == codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo já está spawnado!");
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

        [Command("vlista", "!{#febd0c}USO:~w~ /vlista")]
        public void CMD_vlista(Player player)
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

        [Command("vvender", "!{#febd0c}USO:~w~ /vvender (ID ou nome) (valor)")]
        public void CMD_vvender(Player player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Veiculos
                .Where(x => x.Personagem == p.Codigo && player.Position.DistanceTo(x.Vehicle.Position) <= 2)
                .OrderBy(x => player.Position.DistanceTo(x.Vehicle.Position))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo seu!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor não é válido!");
                return;
            }

            var convite = new Convite()
            {
                Tipo = (int)TipoConvite.VendaVeiculo,
                Personagem = p.Codigo,
                Valor = new string[] { prox.Codigo.ToString(), valor.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == (int)TipoConvite.VendaVeiculo);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você ofereceu seu veículo {prox.Codigo} para {target.NomeIC} por ${valor:N0}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} ofereceu para você o veículo {prox.Codigo} por ${valor:N0}. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.Venda, $"/vvender {prox.Codigo} {valor}", p, target);
        }
    }
}