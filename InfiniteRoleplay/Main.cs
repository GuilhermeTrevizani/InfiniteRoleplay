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

                var banimento = context.Banimentos.FirstOrDefault(x => x.SocialClub == player.SocialClubName);
                if (!Functions.VerificarBanimento(player, banimento))
                    return;

                var user = context.Usuarios.FirstOrDefault(x => x.SocialClubRegistro == player.SocialClubName);
                NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", user?.Nome ?? string.Empty, "");
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
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            Functions.SendMessageToNearbyPlayers(player, message, TipoMensagemJogo.ChatICNormal, player.Dimension > 0 ? 7.5f : 10.0f);
        }
    }
}