﻿using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Cellphone : Script
    {
        [Command("sms", "!{#febd0c}USO:~w~ /sms (número ou nome do contato) (mensagem)", GreedyArg = true)]
        public void CMD_sms(Player player, string numeroNomeContato, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            if (p.TempoPrisao > 0 || p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora!");
                return;
            }

            int.TryParse(numeroNomeContato, out int numero);
            if (numero == 0)
                numero = p.Contatos.FirstOrDefault(x => x.Nome.ToLower().Contains(numeroNomeContato.ToLower()))?.Celular ?? 0;

            if (numero == p.Celular)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode enviar um SMS para você mesmo!");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == numero);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Número indisponível!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F2FF43}" + $"[CELULAR] SMS para {p.ObterNomeContato(numero)}: {mensagem}");
            Functions.SendMessageToNearbyPlayers(player, "envia uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] SMS de {target.ObterNomeContato(p.Celular)}: {mensagem}");
            Functions.SendMessageToNearbyPlayers(target.Player, "recebe uma mensagem de texto.", TipoMensagemJogo.Ame, 5, true);
        }

        [Command("ligar", "!{#febd0c}USO:~w~ /ligar (número ou nome do contato)")]
        public void CMD_ligar(Player player, string numeroNomeContato)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            if (p.TempoPrisao > 0 || p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora!");
                return;
            }

            if (p.NumeroLigacao > 0 || Global.PersonagensOnline.Any(x => x.NumeroLigacao == p.Celular))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está em uma ligação!");
                return;
            }

            int.TryParse(numeroNomeContato, out int numero);
            if (numero == 0)
                numero = p.Contatos.FirstOrDefault(x => x.Nome.ToLower().Contains(numeroNomeContato.ToLower()))?.Celular ?? 0;

            if (numero == p.Celular)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode ligar para você mesmo!");
                return;
            }

            if (numero == 911)
            {
                p.NumeroLigacao = numero;
                p.StatusLigacao = 1;
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.");
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(numero)} diz: Central de emergência, deseja falar com LSPD ou LSFD?");
                return;
            }

            if (numero == 5555555)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.");
                if (Global.PersonagensOnline.Count(x => x.Emprego == TipoEmprego.Taxista && x.IsEmTrabalho) == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(numero)} diz: Desculpe, não temos nenhum taxista em serviço no momento.");
                    return;
                }

                p.NumeroLigacao = numero;
                p.StatusLigacao = 1;
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(numero)} diz: Downtown Cab Company, para onde deseja ir?");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == numero);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Número indisponível!");
                return;
            }

            if (target.NumeroLigacao > 0 || Global.PersonagensOnline.Any(x => x.NumeroLigacao == target.Celular))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] {p.ObterNomeContato(numero)} está ocupado!");
                return;
            }

            p.NumeroLigacao = numero;
            p.StatusLigacao = 0;
            p.TimerCelular = new TagTimer(8000)
            {
                Tag = p.Codigo,
            };
            p.TimerCelular.Elapsed += TimerCelular_Elapsed;
            p.TimerCelular.Start();
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Você está ligando para {p.ObterNomeContato(numero)}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(p.Celular)}. (digite /atender ou /desligar)");
            Functions.SendMessageToNearbyPlayers(target.Player, $"O celular de {target.NomeIC} está tocando.", TipoMensagemJogo.Do, 5, true);
        }

        private void TimerCelular_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var timer = (TagTimer)sender;
            timer.ElapsedCount++;
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == (int)timer.Tag);
            if (p == null)
            {
                timer?.Stop();
                return;
            }

            if (timer.ElapsedCount == 5)
            {
                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Sua ligação para {p.ObterNomeContato(p.NumeroLigacao)} caiu após tocar 5 vezes!");
                p.LimparLigacao();
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao);
            if (target == null)
            {
                Functions.EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Sua ligação para {p.ObterNomeContato(p.NumeroLigacao)} caiu!");
                p.LimparLigacao();
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] O seu celular está tocando! Ligação de {target.ObterNomeContato(p.Celular)}. (digite /atender ou /desligar)");
            Functions.SendMessageToNearbyPlayers(target.Player, $"O celular de {target.NomeIC} está tocando.", TipoMensagemJogo.Do, 5, true);
        }

        [Command("desligar", "!{#febd0c}USO:~w~ /desligar", Alias = "des")]
        public void CMD_desligar(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
            if (target == null && p.NumeroLigacao > 0)
                target = Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao);

            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Seu celular não está tocando ou você não está uma ligação!");
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] {target.ObterNomeContato(p.Celular)} desligou a ligação!");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Você desligou a ligação de {p.ObterNomeContato(target.Celular)}!");

            p.LimparLigacao();
            target.LimparLigacao();
        }

        [Command("atender", "!{#febd0c}USO:~w~ /atender")]
        public void CMD_atender(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            if (p.TempoPrisao > 0 || p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora!");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
            if (target == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Seu celular não está tocando!");
                return;
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Sua ligação para {target.ObterNomeContato(p.Celular)} foi atendida!");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Você atendeu a ligação de {p.ObterNomeContato(target.Celular)}!");

            target.StatusLigacao = 1;
            target.LimparLigacao(true);
        }

        [Command("celular", "!{#febd0c}USO:~w~ /cel", Alias = "cel")]
        public void CMD_cel(Player player) => Functions.AbrirCelular(player, string.Empty, 0);

        [Command("gps", "!{#febd0c}USO:~w~ /gps (propriedade)")]
        public void CMD_gps(Player player, int propriedade)
        {
            var p = Functions.ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            if (p.TempoPrisao > 0 || p.Algemado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == propriedade);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {propriedade} não existe!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "setWaypoint", prop.EntradaPosX, prop.EntradaPosY);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F2FF43}" + $"[CELULAR] Propriedade {propriedade} foi marcada no GPS.");
        }
    }
}