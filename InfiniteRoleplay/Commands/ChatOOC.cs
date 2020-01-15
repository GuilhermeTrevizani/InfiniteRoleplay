﻿using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class ChatOOC : Script
    {
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

        [Command("pm", GreedyArg = true)]
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