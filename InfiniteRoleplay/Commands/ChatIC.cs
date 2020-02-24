using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class ChatIC : Script
    {
        [Command("me", "!{#febd0c}USO:~w~ /me (mensagem)", GreedyArg = true)]
        public void CMD_me(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Me, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("do", "!{#febd0c}USO:~w~ /do (mensagem)", GreedyArg = true)]
        public void CMD_do(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Do, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("g", "!{#febd0c}USO:~w~ /g (mensagem)", GreedyArg = true)]
        public void CMD_g(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICGrito, 30.0f);
        }

        [Command("baixo", "!{#febd0c}USO:~w~ /baixo (mensagem)", GreedyArg = true)]
        public void CMD_baixo(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICBaixo, player.Dimension > 0 ? 3.75f : 5);
        }

        [Command("s", "!{#febd0c}USO:~w~ /s (ID ou nome) (mensagem)", GreedyArg = true)]
        public void CMD_s(Player player, string idNome, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
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

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F2FF43}" + $"{p.NomeIC} sussura: {mensagem}");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"{p.NomeIC} sussura: {mensagem}");
            Functions.SendMessageToNearbyPlayers(player, $"sussurra algo para {target.NomeIC}.", TipoMensagemJogo.Ame, 5, true);
        }
    }
}