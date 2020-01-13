using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Timers;

namespace InfiniteRoleplay
{
    public class Main : Script
    {
        Timer timerPrincipal { get; set; }

        [ServerEvent(Event.ResourceStart)]
        public void ResourceStart()
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = CultureInfo.DefaultThreadCurrentUICulture =
                  CultureInfo.GetCultureInfo("pt-BR");

            NAPI.Util.ConsoleOutput("Infinite Roleplay por Guilherme Trevizani (TR3V1Z4)");

            NAPI.Server.SetAutoRespawnAfterDeath(false);
            NAPI.Server.SetGlobalServerChat(false);
            NAPI.Server.SetAutoSpawnOnConnect(false);
            NAPI.World.SetWeather(Weather.CLEAR);
            NAPI.World.SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            NAPI.Server.SetCommandErrorMessage("!{#FF6A4D}<!>~w~ O comando informado não existe!");

            string host = NAPI.Resource.GetSetting<string>(this, "db_host");
            string db = NAPI.Resource.GetSetting<string>(this, "db_database");
            string user = NAPI.Resource.GetSetting<string>(this, "db_username");
            string pass = NAPI.Resource.GetSetting<string>(this, "db_password");

            Global.ConnectionString = $"Server={host};Database={db};Uid={user};Pwd={pass}";

            using (var context = new RoleplayContext())
            {
                context.Database.ExecuteSqlCommand("UPDATE Personagens SET Online=0");
                NAPI.Util.ConsoleOutput("Status online dos personagens limpo");

                Global.Parametros = context.Parametros.FirstOrDefault();
                NAPI.Util.ConsoleOutput("Parametros carregados");

                Global.Blips = context.Blips.ToList();
                foreach (var b in Global.Blips)
                    b.CriarIdentificador();
                NAPI.Util.ConsoleOutput($"Blips: {Global.Blips.Count}");

                Global.Faccoes = context.Faccoes.ToList();
                NAPI.Util.ConsoleOutput($"Faccoes: {Global.Faccoes.Count}");

                Global.Ranks = context.Ranks.ToList();
                NAPI.Util.ConsoleOutput($"Ranks: {Global.Ranks.Count}");

                Global.Propriedades = context.Propriedades.ToList();
                foreach (var p in Global.Propriedades)
                    p.CriarIdentificador();
                NAPI.Util.ConsoleOutput($"Propriedades: {Global.Propriedades.Count}");

                Global.Precos = context.Precos.ToList();
                NAPI.Util.ConsoleOutput($"Precos: {Global.Precos.Count}");

                Global.Pontos = context.Pontos.ToList();
                foreach (var p in Global.Pontos)
                    p.CriarIdentificador();
                NAPI.Util.ConsoleOutput($"Pontos: {Global.Pontos.Count}");
            }

            Global.PersonagensOnline = new List<Personagem>();
            Global.Veiculos = new List<Veiculo>();

            timerPrincipal = new Timer(60000);
            timerPrincipal.Elapsed += TimerPrincipal_Elapsed;
            timerPrincipal.Start();
        }

        private void TimerPrincipal_Elapsed(object sender, ElapsedEventArgs e)
        {
            NAPI.World.SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            foreach (var p in NAPI.Pools.GetAllPlayers())
                Functions.SalvarPersonagem(p);
        }

        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            timerPrincipal?.Stop();
        }

        [ServerEvent(Event.PlayerConnected)]
        public void OnPlayerConnected(Client player)
        {
            player.Name = player.SocialClubName;
            player.Dimension = (uint)new Random().Next(1, 1000);

            using (var context = new RoleplayContext())
            {
                if (!context.Whitelist.Any(x => x.SocialClub == player.SocialClubName))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "mensagem", $"Sua SocialClub ({player.SocialClubName}) não está na whitelist. Solicite em: <input type='text' class='form-control' value='https://discord.gg/53VHCrF'>");
                    player.Kick();
                    return;
                }

                if (!Functions.VerificarBanimento(player, context.Banimentos.FirstOrDefault(x => x.SocialClub == player.SocialClubName)))
                    return;

                NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", context.Usuarios.FirstOrDefault(x => x.SocialClubRegistro == player.SocialClubName)?.Nome ?? string.Empty, "");
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            Functions.SalvarPersonagem(player, false);
            Global.PersonagensOnline.RemoveAll(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client player, Client killer, uint reason)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            NAPI.Task.Run(() =>
            {
                player.RemoveAllWeapons();
                player.Dimension = 0;
                NAPI.Player.SpawnPlayer(player, new Vector3(343.8652, -1399.016, 32.50928));
            }, delayTime: 5000);
        }

        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client player, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.StatusLigacao > 0)
            {
                Functions.EnviarMensagemCelular(p, Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao), message);

                if (p.NumeroLigacao == 911)
                {
                    if (p.StatusLigacao == 1)
                    {
                        if (message.ToUpper().Contains("LSPD"))
                            p.ExtraLigacao = "LSPD";
                        else if (message.ToUpper().Contains("LSFD"))
                            p.ExtraLigacao = "LSFD";

                        if (string.IsNullOrWhiteSpace(p.ExtraLigacao))
                        {
                            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: Não entendi sua mensagem. Deseja falar com LSPD ou LSFD?");
                            return;
                        }

                        p.StatusLigacao = 2;
                        Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: {p.ExtraLigacao}, qual sua emergência?");
                        return;
                    }

                    if (p.StatusLigacao == 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: Nossas unidades foram alertadas!");

                        var tipoFaccao = p.ExtraLigacao == "LSPD" ? TipoFaccao.Policial : TipoFaccao.Medica;
                        Functions.EnviarMensagemTipoFaccao(tipoFaccao, "Ligação 911", true, true);
                        Functions.EnviarMensagemTipoFaccao(tipoFaccao, $"De: ~w~{p.Celular}", true, true);
                        Functions.EnviarMensagemTipoFaccao(tipoFaccao, $"Mensagem: ~w~{message}", true, true);

                        p.LimparLigacao();
                    }
                }
                return;
            }

            var targetLigacao = Global.PersonagensOnline.FirstOrDefault(x => x.StatusLigacao > 0 && x.NumeroLigacao == p.Celular);
            if (targetLigacao != null)
            {
                Functions.EnviarMensagemCelular(p, targetLigacao, message);
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, message, TipoMensagemJogo.ChatICNormal, player.Dimension > 0 ? 7.5f : 10.0f);
        }
    }
}