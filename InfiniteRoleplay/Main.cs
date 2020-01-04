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

            NAPI.Server.SetCommandErrorMessage("~r~<!>~w~ O comando informado não existe!");

            using (var context = new RoleplayContext())
            {
                context.Database.ExecuteSqlCommand("UPDATE Personagens SET Online=0");
                NAPI.Util.ConsoleOutput("Status online dos personagens limpo");

                Global.Blips = context.Blips.ToList();
                foreach(var b in Global.Blips)
                {
                    var blip = NAPI.Blip.CreateBlip(new Vector3(b.PosX, b.PosY, b.PosZ));
                    blip.Sprite = (uint)b.Tipo;
                    blip.Color = b.Cor;
                    blip.Name = Functions.ObterNomePadraoBlip(b.Tipo, b.Nome);
                    blip.SetData(nameof(b.Codigo), b.Codigo);
                }
                NAPI.Util.ConsoleOutput($"Blips: {Global.Blips.Count}");

                Global.Faccoes = context.Faccoes.ToList();
                NAPI.Util.ConsoleOutput($"Faccções: {Global.Faccoes.Count}");

                Global.Ranks = context.Ranks.ToList();
                NAPI.Util.ConsoleOutput($"Ranks: {Global.Ranks.Count}");
            }

            Global.PersonagensOnline = new List<Personagem>();

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
            NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected");
            player.Dimension = 1;
            player.Position = new Vector3(-74.6524, -818.8309, 326.1756);

            using (var context = new RoleplayContext())
            {
                if (context.Whitelist.Any(x => x.SocialClub == player.SocialClubName))
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "Digite /reg para registrar ou /log para logar se já possuir um usuário!");
                else
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Sua SocialClub ({player.SocialClubName}) não está na whitelist. Solicite em https://discord.gg/53VHCrF");
            }
        }

        [ServerEvent(Event.PlayerDisconnected)]
        public void OnPlayerDisconnected(Client player, DisconnectionType type, string reason)
        {
            Functions.SalvarPersonagem(player, false);
            Global.PersonagensOnline.RemoveAll(x => x.UsuarioBD.SocialClub == player.SocialClubName);
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
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            Functions.SendMessageToNearbyPlayers(player, message, TipoMensagemJogo.ChatICNormal, player.Dimension > 0 ? 7.5f : 10.0f);
        }
    }
}