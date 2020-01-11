using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class Chat : Script
    {
        [Command("me", GreedyArg = true)]
        public void CMD_me(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Me, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("do", GreedyArg = true)]
        public void CMD_do(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Do, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("g", GreedyArg = true)]
        public void CMD_g(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICGrito, 35.0f);
        }

        [Command("b", GreedyArg = true)]
        public void CMD_b(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatOOC, player.Dimension > 0 ? 5.0f : 10.0f);
        }

        [Command("baixo", GreedyArg = true)]
        public void CMD_baixo(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.ChatICBaixo, player.Dimension > 0 ? 3.75f : 5);
        }

        [Command("/s", GreedyArg = true)]
        public void CMD_s(Client player, string idNome, string mensagem)
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

            float distance = player.Position.DistanceTo(target.Player.Position);
            if (distance > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F2FF43}" + $"{Functions.ObterNomeIC(p)} sussura: {mensagem}");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"{Functions.ObterNomeIC(p)} sussura: {mensagem}");
        }

        [Command("/pm", GreedyArg = true)]
        public void CMD_pm(Client player, string idNome, string mensagem)
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

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F2FF43}" + $"(( PM para {target.Nome} [{target.ID}]: {mensagem} ))");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"(( PM de {p.Nome} [{p.ID}]: {mensagem} ))");
        }
    }
}