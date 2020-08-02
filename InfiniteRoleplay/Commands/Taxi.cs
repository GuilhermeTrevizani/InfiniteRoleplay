using GTANetworkAPI;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Taxi : Script
    {
        [Command("taxiduty", "!{#febd0c}USO:~w~ /taxiduty")]
        public void CMD_taxiduty(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é um taxista!");
                return;
            }

            p.IsEmTrabalho = !p.IsEmTrabalho;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.IsEmTrabalho ? "entrou em" : "saiu de")} serviço como taxista!");
        }

        [Command("taxicha", "!{#febd0c}USO:~w~ /taxicha")]
        public void CMD_taxicha(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista!");
                return;
            }

            if (player.Vehicle?.Model != (uint)VehicleHash.Taxi)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi!");
                return;
            }

            var chamadas = Global.PersonagensOnline.Where(x => x.AguardandoTipoServico == (int)TipoEmprego.Taxista).OrderBy(x => x.Codigo).ToList();
            if (chamadas.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada para taxistas!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Chamadas Aguardando Taxistas");
            foreach (var c in chamadas)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Chamada #{c.Codigo}");
        }

        [Command("taxiac", "!{#febd0c}USO:~w~ /taxiac (chamada)")]
        public void CMD_taxiac(Player player, int chamada)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Emprego != TipoEmprego.Taxista || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em serviço como taxista!");
                return;
            }

            if (player.Vehicle?.Model != (uint)VehicleHash.Taxi)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um taxi!");
                return;
            }
            
            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == chamada && x.AguardandoTipoServico == (int)TipoEmprego.Taxista);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhuma chamada com esse código!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "setWaypoint", target.Player.Position.X, target.Player.Position.Y);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você está atendendo a chamada {chamada} e a localização do solicitante foi marcada em seu GPS!");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] SMS de {p.ObterNomeContato(5555555)}: Nosso taxista {p.Nome} está atendendo sua chamada! Placa: {player.Vehicle.NumberPlate}");
        }
    }
}