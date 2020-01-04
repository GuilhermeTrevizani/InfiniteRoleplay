using GTANetworkAPI;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Faction : Script
    {
        [Command("f", GreedyArg = true)]
        public void CMD_f(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            if (p.FaccaoBD.ChatBloqueado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Chat da facção está bloqueado!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao);
            foreach (var pl in players)
                pl.Player.SendChatMessage("!{#" + p.FaccaoBD.Cor + "}" + $"(( {p.RankBD.Nome} {p.Nome} [{p.ID}]: {mensagem} ))");
        }

        [Command("membros")]
        public void CMD_membros(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao).OrderByDescending(x => x.Rank).ThenBy(x => x.Nome);
            player.SendChatMessage("!{#" + p.FaccaoBD.Cor + "}" + p.FaccaoBD.Nome);
            foreach (var pl in players)
                player.SendChatMessage($"{pl.RankBD.Nome} {pl.Nome} [{pl.ID}] (( {pl.UsuarioBD.Nome} ))");
        }

        [Command("blockf")]
        public void CMD_blockf(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(p.FaccaoBD)].ChatBloqueado = !p.FaccaoBD.ChatBloqueado;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!p.FaccaoBD.ChatBloqueado ? "des" : string.Empty)}bloqueou o chat da facção!");
        }
    }
}