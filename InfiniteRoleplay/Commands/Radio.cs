﻿using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class Radio : Script
    {
        [Command("canal")]
        public void CMD_canal(Client player, int slot, int canal)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.CanalRadio == -1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um rádio!");
                return;
            }

            if (slot < 1 || slot > 3)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Slot deve ser entre 1 e 3!");
                return;
            }

            if (canal == 911 && p.FaccaoBD?.Tipo != (int)TipoFaccao.Policial)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 911 é reservado para facções policiais!");
                return;
            }

            if (canal == 912 && p.FaccaoBD?.Tipo != (int)TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 912 é reservado para facções médicas!");
                return;
            }

            if (slot == 1)
                p.CanalRadio = canal;
            else if (slot == 2)
                p.CanalRadio2 = canal;
            else if (slot == 3)
                p.CanalRadio3 = canal;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou seu canal de rádio do slot {slot} para {canal}!");
        }

        [Command("r", GreedyArg = true)]
        public void CMD_r(Client player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 1, mensagem);

        [Command("r2", GreedyArg = true)]
        public void CMD_r2(Client player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 2, mensagem);

        [Command("r3", GreedyArg = true)]
        public void CMD_r3(Client player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 3, mensagem);
    }
}