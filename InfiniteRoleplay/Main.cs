﻿using GTANetworkAPI;
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

            var host = NAPI.Resource.GetSetting<string>(this, "db_host");
            var db = NAPI.Resource.GetSetting<string>(this, "db_database");
            var user = NAPI.Resource.GetSetting<string>(this, "db_username");
            var pass = NAPI.Resource.GetSetting<string>(this, "db_password");

            Global.ConnectionString = $"Server={host};Database={db};User ID={user};Password={pass}";

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

                Global.Armarios = context.Armarios.ToList();
                foreach (var a in Global.Armarios)
                    a.CriarIdentificador();
                NAPI.Util.ConsoleOutput($"Armarios: {Global.Armarios.Count}");

                Global.ArmariosItens = context.ArmariosItens.ToList();
                NAPI.Util.ConsoleOutput($"ArmariosItens: {Global.ArmariosItens.Count}");
            }

            Functions.CarregarSkins();
            NAPI.Util.ConsoleOutput($"Skins: {Global.Skins.Count}");

            Functions.CarregarConcessionarias();
            NAPI.Util.ConsoleOutput($"Concessionarias: {Global.Concessionarias.Count}");

            Functions.CarregarEmpregos();
            NAPI.Util.ConsoleOutput($"Empregos: {Global.Empregos.Count}");

            Global.PersonagensOnline = new List<Personagem>();
            Global.Veiculos = new List<Veiculo>();

            NAPI.TextLabel.CreateTextLabel("Prisão", Constants.PosicaoPrisao, 5, 2, 0, new Color(254, 189, 12));
            NAPI.TextLabel.CreateTextLabel("Use /prender", new Vector3(Constants.PosicaoPrisao.X, Constants.PosicaoPrisao.Y, Constants.PosicaoPrisao.Z - 0.1), 5, 1, 0, new Color(255, 255, 255));

            timerPrincipal = new Timer(60000);
            timerPrincipal.Elapsed += TimerPrincipal_Elapsed;
            timerPrincipal.Start();
        }

        private void TimerPrincipal_Elapsed(object sender, ElapsedEventArgs e)
        {
            NAPI.World.SetTime(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);

            foreach (var p in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                Functions.SalvarPersonagem(p);
        }

        [ServerEvent(Event.ResourceStop)]
        public void ResourceStop()
        {
            timerPrincipal?.Stop();
            foreach (var p in Global.PersonagensOnline.Where(x => x.Codigo > 0))
                Functions.SalvarPersonagem(p);
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
            var p = Functions.ObterPersonagem(player);
            if (p?.Codigo > 0)
            {
                Functions.GravarLog(TipoLog.Saida, $"Tipo: {(int)type} | Razão: {reason}", p, null);
                Functions.SalvarPersonagem(p, false);
            }

            Global.PersonagensOnline.RemoveAll(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
        }

        [ServerEvent(Event.PlayerDeath)]
        public void OnPlayerDeath(Client player, Client killer, uint reason)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            Functions.GravarLog(TipoLog.Morte, reason.ToString(), p, Functions.ObterPersonagem(killer));

            NAPI.Task.Run(() =>
            {
                player.RemoveAllWeapons();
                player.Dimension = 0;

                var pos = new Vector3(343.8652, -1399.016, 32.50928);
                if (p.TempoPrisao > 0)
                {
                    using (var context = new RoleplayContext())
                    {
                        var prisao = context.Prisoes.LastOrDefault(x => x.Preso == p.Codigo);
                        if (prisao.Cela == 1)
                            pos = new Vector3(460.4085, -994.0992, 25);
                        else if (prisao.Cela == 2)
                            pos = new Vector3(460.4085, -997.7994, 25);
                        else if (prisao.Cela == 3)
                            pos = new Vector3(460.4085, -1001.342, 25);
                    }
                }

                NAPI.Player.SpawnPlayer(player, pos);
            }, delayTime: 5000);
        }

        [ServerEvent(Event.ChatMessage)]
        public void OnChatMessage(Client player, string message) => Functions.EnviarMensagemChat(Functions.ObterPersonagem(player), message, TipoMensagemJogo.ChatICNormal);
    }
}