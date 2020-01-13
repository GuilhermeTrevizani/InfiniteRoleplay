﻿using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Staff : Script
    {
        #region Staff 1
        [Command("ir")]
        public void CMD_ir(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            var pos = target.Player.Position;
            pos.X += 2;
            player.Position = pos;
            player.Dimension = target.Player.Dimension;
        }

        [Command("trazer")]
        public void CMD_trazer(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            var pos = player.Position;
            pos.X += 2;
            target.Player.Position = pos;
            target.Player.Dimension = player.Dimension;
        }

        [Command("tp")]
        public void CMD_tp(Client player, string idNome, string idNomeDestino)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            var targetDest = Functions.ObterPersonagemPorIdNome(player, idNomeDestino, false);
            if (targetDest == null)
                return;

            var pos = targetDest.Player.Position;
            pos.X += 2;
            target.Player.Position = pos;
            target.Player.Dimension = targetDest.Player.Dimension;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} teleportou você para {targetDest.Nome}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você teleportou {target.Nome} para {targetDest.Nome}.");
        }

        [Command("vw")]
        public void CMD_vw(Client player, string idNome, uint vw)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Dimension = vw;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua dimensão para {vw}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a dimensão de {target.Nome} para {vw}.");
        }

        [Command("a", GreedyArg = true)]
        public void CMD_a(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.UsuarioBD.Staff >= 1))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#33EE33}" + $"(( [STAFF {p.UsuarioBD.Staff}] {p.UsuarioBD.Nome}: {mensagem} ))");
        }

        [Command("o", GreedyArg = true)]
        public void CMD_o(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline)
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#AAC4E5}" + $"(( {p.UsuarioBD.Nome}: {mensagem} ))");
        }

        [Command("kick", GreedyArg = true)]
        public void CMD_kick(Client player, string idNome, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            using (var context = new RoleplayContext())
            {
                context.Punicoes.Add(new Entities.Punicao()
                {
                    Data = DateTime.Now,
                    Duracao = 0,
                    Motivo = motivo,
                    Personagem = target.Codigo,
                    Tipo = (int)TipoPunicao.Kick,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();
            }

            Functions.EnviarMensagem(target.Player, TipoMensagem.Punicao, $"{p.UsuarioBD.Nome} kickou você. Motivo: {motivo}");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você kickou {target.Nome}. Motivo: {motivo}");
            target.Player.Kick();
        }
        
        [Command("irveh")]
        public void CMD_irveh(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está spawnado!");
                return;
            }

            var pos = veh.Vehicle.Position;
            pos.X += 5;
            player.Position = pos;
            player.Dimension = veh.Vehicle.Dimension;
        }

        [Command("trazerveh")]
        public void CMD_trazerveh(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está spawnado!");
                return;
            }

            var pos = player.Position;
            pos.X += 5;
            veh.Vehicle.Position = pos;
            veh.Vehicle.Dimension = player.Dimension;
        }
        #endregion Staff 1

        #region Staff 2
        [Command("vida")]
        public void CMD_vida(Client player, string idNome, int vida)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (vida < 1 || vida > 100)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Vida deve ser entre 1 e 100.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Health = vida;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua vida para {vida}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a vida de {target.Nome} para {vida}.");
            Functions.GravarLog(TipoLog.Staff, $"/vida {vida}", p, target);
        }

        [Command("colete")]
        public void CMD_colete(Client player, string idNome, int colete)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (colete < 1 || colete > 100)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Colete deve ser entre 1 e 100.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.Armor = colete;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu colete para {colete}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o colete de {target.Nome} para {colete}.");
            Functions.GravarLog(TipoLog.Staff, $"/colete {colete}", p, target);
        }

        [Command("skin")]
        public void CMD_skin(Client player, string idNome, string skin)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            var pedHash = NAPI.Util.PedNameToModel(skin);
            if (pedHash == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Skin {skin} não existe!");
                return;
            }

            target.Player.SetSkin(pedHash);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua skin para {pedHash.ToString()}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a skin de {target.Nome} para {pedHash.ToString()}.");
        }

        [Command("skinc")]
        public void CMD_skinc(Client player, string idNome, int slot, int drawable, int texture)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.SetClothes(slot, drawable, texture);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou sua roupa no slot {slot} para desenho {drawable} e textura {texture}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a roupa de {target.Nome} no slot {slot} para desenho {drawable} e textura {texture}.");
        }

        [Command("skina")]
        public void CMD_skina(Client player, string idNome, int slot, int drawable, int texture)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Player.SetAccessories(slot, drawable, texture);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu acessório no slot {slot} para desenho {drawable} e textura {texture}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o acessório de {target.Nome} no slot {slot} para desenho {drawable} e textura {texture}.");
        }

        [Command("checar")]
        public void CMD_checar(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            Functions.MostrarStats(player, target);
        }

        [Command("ban", GreedyArg = true)]
        public void CMD_ban(Client player, string idNome, int dias, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            using (var context = new RoleplayContext())
            {
                var ban = new Entities.Banimento()
                {
                    Data = DateTime.Now,
                    Expiracao = null,
                    Motivo = motivo,
                    Usuario = target.UsuarioBD.Codigo,
                    SocialClub = target.UsuarioBD.SocialClubRegistro,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                };

                if (dias > 0)
                    ban.Expiracao = DateTime.Now.AddDays(dias);

                context.Banimentos.Add(ban);

                context.Punicoes.Add(new Entities.Punicao()
                {
                    Data = DateTime.Now,
                    Duracao = dias,
                    Motivo = motivo,
                    Personagem = target.Codigo,
                    Tipo = (int)TipoPunicao.Ban,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();
            }

            var strBan = dias == 0 ? "permanentemente" : $"por {dias} dia{(dias > 1 ? "s" : string.Empty)}";
            Functions.EnviarMensagem(target.Player, TipoMensagem.Punicao, $"{p.UsuarioBD.Nome} baniu você {strBan}. Motivo: {motivo}");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você baniu {target.Nome} {strBan}. Motivo: {motivo}");

            target.Player.Kick();
        }

        [Command("banoff", GreedyArg = true)]
        public void CMD_banoff(Client player, int personagem, int dias, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var per = context.Personagens.FirstOrDefault(x => x.Codigo == personagem);
                if (per == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {personagem} não existe!");
                    return;
                }

                var user = context.Usuarios.FirstOrDefault(x => x.Codigo == per.Usuario);

                var ban = new Entities.Banimento()
                {
                    Data = DateTime.Now,
                    Expiracao = null,
                    Motivo = motivo,
                    Usuario = user.Codigo,
                    SocialClub = user.SocialClubRegistro,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                };

                if (dias > 0)
                    ban.Expiracao = DateTime.Now.AddDays(dias);

                context.Banimentos.Add(ban);

                context.Punicoes.Add(new Entities.Punicao()
                {
                    Data = DateTime.Now,
                    Duracao = dias,
                    Motivo = motivo,
                    Personagem = per.Codigo,
                    Tipo = (int)TipoPunicao.Ban,
                    UsuarioStaff = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();

                var strBan = dias == 0 ? "permanentemente" : $"por {dias} dia{(dias > 1 ? "s" : string.Empty)}";
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você baniu {per.Nome} {strBan}. Motivo: {motivo}");
            }
        }

        [Command("unban")]
        public void CMD_unban(Client player, int usuario)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var ban = context.Banimentos.FirstOrDefault(x => x.Usuario == usuario);
                if (ban == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Usuário {usuario} não está banido!");
                    return;
                }

                context.Banimentos.Remove(ban);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você desbaniu o usuário {usuario}!");
            Functions.GravarLog(TipoLog.Staff, $"/unban {usuario}", p, null);
        }
        #endregion Staff 2

        #region Staff 1337
        [Command("gmx")]
        public void CMD_gmx(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            foreach (var pl in NAPI.Pools.GetAllPlayers())
            {
                Functions.EnviarMensagem(pl, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} reiniciará o servidor.");
                Functions.SalvarPersonagem(pl);
            }
        }

        [Command("tempo")]
        public void CMD_tempo(Client player, int tempo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (tempo < 0 || tempo > 13)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tempo deve ser entre 0 e 13.");
                return;
            }

            NAPI.World.SetWeather((Weather)tempo);
        }

        [Command("proximo")]
        public void CMD_proximo(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            bool isTemAlgoProximo = false;
            float distanceVer = 5f;

            foreach (var b in Global.Blips)
            {
                float distance = player.Position.DistanceTo(new Vector3(b.PosX, b.PosY, b.PosZ));
                if (distance <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Blip {b.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var prop in Global.Propriedades)
            {
                float distance = player.Position.DistanceTo(new Vector3(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ));
                if (distance <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Propriedade {prop.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var ponto in Global.Pontos)
            {
                float distance = player.Position.DistanceTo(new Vector3(ponto.PosX, ponto.PosY, ponto.PosZ));
                if (distance <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Ponto {ponto.Codigo}");
                    isTemAlgoProximo = true;
                }
            }

            if (!isTemAlgoProximo)
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum item!");
        }

        [Command("cblip", GreedyArg = true)]
        public void CMD_cblip(Client player, int tipo, int cor, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (tipo < 0 || tipo > 744)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo deve ser entre 1 e 744.");
                return;
            }

            if (cor < 0 || cor > 85)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cor deve ser entre 1 e 85.");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            var blip = new Entities.Blip()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Cor = cor,
                Tipo = tipo,
                Nome = nome,
            };

            using (var context = new RoleplayContext())
            {
                context.Blips.Add(blip);
                context.SaveChanges();
            }

            blip.CriarIdentificador();

            Global.Blips.Add(blip);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} criado com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/cblip {blip.Codigo}", p, null);
        }

        [Command("rblip")]
        public void CMD_rblip(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Codigo == codigo);
            if (blip == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Blip {codigo} não existe!");
                return;
            }
            using (var context = new RoleplayContext())
                context.Database.ExecuteSqlCommand($"DELETE FROM Blips WHERE Codigo = {codigo}");

            blip.DeletarIdentificador();

            Global.Blips.Remove(blip);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Blip {blip.Codigo} removido com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/rblip {blip.Codigo}", p, null);
        }

        [Command("addwhite")]
        public void CMD_addwhite(Client player, string socialClub)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Whitelist.Any(x => x.SocialClub == socialClub))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"{socialClub} já está na whitelist!");
                    return;
                }

                context.Whitelist.Add(new Entities.Whitelist()
                {
                    SocialClub = socialClub,
                    Data = DateTime.Now,
                    Usuario = p.UsuarioBD.Codigo,
                });
                context.SaveChanges();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você adicionou {socialClub} na whitelist!");
            }

            Functions.GravarLog(TipoLog.Staff, $"/addwhite {socialClub}", p, null);
        }

        [Command("delwhite")]
        public void CMD_delwhite(Client player, string socialClub)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var whiteList = context.Whitelist.FirstOrDefault(x => x.SocialClub == socialClub);
                if (whiteList == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"{socialClub} não está na whitelist!");
                    return;
                }

                context.Whitelist.Remove(whiteList);
                context.SaveChanges();

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu {socialClub} da whitelist!");
            }

            Functions.GravarLog(TipoLog.Staff, $"/delwhite {socialClub}", p, null);
        }

        [Command("cfac", GreedyArg = true)]
        public void CMD_cfac(Client player, int tipo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoFaccao), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            var faccao = new Entities.Faccao()
            {
                Nome = nome,
                Cor = "FFFFFF",
                Tipo = tipo,
            };

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Add(faccao);
                context.SaveChanges();
            }

            Global.Faccoes.Add(faccao);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você criou a facção {faccao.Codigo}!");

            Functions.GravarLog(TipoLog.Staff, $"/cfac {faccao.Codigo}", p, null);
        }

        [Command("efacnome", GreedyArg = true)]
        public void CMD_efacnome(Client player, int codigo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Nome = nome;

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o nome da facção {faccao.Codigo} para {nome}!");
            Functions.GravarLog(TipoLog.Staff, $"/efacnome {faccao.Codigo} {nome}", p, null);
        }

        [Command("efactipo")]
        public void CMD_efactipo(Client player, int codigo, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoFaccao), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Tipo = tipo;

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o tipo da facção {faccao.Codigo} para {tipo}!");
            Functions.GravarLog(TipoLog.Staff, $"/efactipo {faccao.Codigo} {tipo}", p, null);
        }

        [Command("efaccor")]
        public void CMD_efaccor(Client player, int codigo, string cor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            if (cor.Length > 6)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cor deve ter até 6 caracteres.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Cor = cor;

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a cor da facção {faccao.Codigo} para {cor}!");
            Functions.GravarLog(TipoLog.Staff, $"/efaccor {faccao.Codigo} {cor}", p, null);
        }

        [Command("efacrankgestor")]
        public void CMD_efacrankgestor(Client player, int codigo, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            if (rank <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank deve ser maior que 0.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankGestor = rank;

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o rank gestor da facção {faccao.Codigo} para {rank}!");
            Functions.GravarLog(TipoLog.Staff, $"/efacrankgestor {faccao.Codigo} {rank}", p, null);
        }

        [Command("efacranklider")]
        public void CMD_efacranklider(Client player, int codigo, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            if (rank <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank deve ser maior que 0.");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankLider = rank;

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o rank líder da facção {faccao.Codigo} para {rank}!");
            Functions.GravarLog(TipoLog.Staff, $"/efacranklider {faccao.Codigo} {rank}", p, null);
        }

        [Command("rfac")]
        public void CMD_rfac(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == codigo);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {codigo} não existe!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Personagens.Any(x => x.Faccao == codigo))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não é possível remover a facção {codigo} pois existem personagens nela!");
                    return;
                }

                context.Database.ExecuteSqlCommand($"DELETE FROM Faccoes WHERE Codigo = {codigo}");
                context.Database.ExecuteSqlCommand($"DELETE FROM `Ranks` WHERE Faccao = {codigo}");
            }

            Global.Faccoes.Remove(faccao);
            Global.Ranks.RemoveAll(x => x.Faccao == codigo);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu a facção {faccao.Codigo}!");
            Functions.GravarLog(TipoLog.Staff, $"/rfac {faccao.Codigo}", p, null);
        }

        [Command("faccoes")]
        public void CMD_faccoes(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (Global.Faccoes.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não existe nenhuma facção!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "Lista de Facções");
            foreach (var f in Global.Faccoes)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#" + f.Cor + "}" + $"{f.Nome} [{f.Codigo}]");
        }

        [Command("crank", GreedyArg = true)]
        public void CMD_crank(Client player, int fac, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (!(p?.UsuarioBD?.Staff >= 1337 || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe!");
                return;
            }

            var rank = new Entities.Rank()
            {
                Faccao = fac,
                Nome = nome,
            };

            using (var context = new RoleplayContext())
            {
                var ranks = context.Ranks.Where(x => x.Faccao == fac).ToList();
                rank.Codigo = ranks.Count == 0 ? 1 : ranks.Max(x => x.Codigo) + 1;
                context.Ranks.Add(rank);
                context.SaveChanges();
            }

            Global.Ranks.Add(rank);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você criou o rank {rank.Nome} [{rank.Codigo}] da facção {faction.Nome} [{faction.Codigo}]!");
            Functions.GravarLog(TipoLog.Staff, $"/crank {faction.Codigo} {rank.Codigo}", p, null);
        }

        [Command("rrank")]
        public void CMD_rrank(Client player, int fac, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (!(p?.UsuarioBD?.Staff >= 1337 || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} da facção {fac} não existe!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Personagens.Any(x => x.Faccao == fac && x.Rank == rank))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não é possível remover o rank {rank} da facção {fac} pois existem personagens nele!");
                    return;
                }

                context.Ranks.Remove(rk);
                context.SaveChanges();
            }

            Global.Ranks.Remove(rk);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você removeu o rank {rk.Nome} [{rk.Codigo}] da facção {fac}!");
            Functions.GravarLog(TipoLog.Staff, $"/rrank {fac} {rk.Codigo}", p, null);
        }

        [Command("erank", GreedyArg = true)]
        public void CMD_erank(Client player, int fac, int rank, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (!(p?.UsuarioBD?.Staff >= 1337 || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} da facção {fac} não existe!");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            Global.Ranks[Global.Ranks.IndexOf(rk)].Nome = nome;

            using (var context = new RoleplayContext())
            {
                context.Ranks.Update(Global.Ranks[Global.Ranks.IndexOf(rk)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o nome do rank {rank} da facção {fac} para {nome}!");
            Functions.GravarLog(TipoLog.Staff, $"/erank {fac} {rank} {nome}", p, null);
        }

        [Command("ranks")]
        public void CMD_ranks(Client player, int fac)
        {
            var p = Functions.ObterPersonagem(player);
            if (!(p?.UsuarioBD?.Staff >= 1337 || (p.Faccao == fac && p.Rank >= p.FaccaoBD.RankLider)))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var faction = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faction == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe!");
                return;
            }

            var ranks = Global.Ranks.Where(x => x.Faccao == fac).OrderBy(x => x.Codigo).ToList();
            if (ranks.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Não existe nenhum rank para a facção {fac}!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#" + faction.Cor + "}" + $"{faction.Nome} [{faction.Codigo}]");
            foreach (var r in ranks)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{r.Nome} [{r.Codigo}]");
        }

        [Command("staff")]
        public void CMD_staff(Client player, string idNome, int staff)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            using (var context = new RoleplayContext())
                context.Database.ExecuteSqlCommand($"UPDATE Usuarios SET Staff = {staff} WHERE Codigo = {target.Usuario}");

            target.UsuarioBD.Staff = staff;
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu nível staff para {staff}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o nível staff de {target.UsuarioBD.Nome} para {staff}.");
            Functions.GravarLog(TipoLog.Staff, $"/staff {staff}", p, target);
        }

        [Command("lider")]
        public void CMD_lider(Client player, string idNome, int fac)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            var faccao = Global.Faccoes.FirstOrDefault(x => x.Codigo == fac);
            if (faccao == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Facção {fac} não existe!");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == fac && x.Codigo == faccao.RankLider);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank líder ({faccao.RankLider}) da facção {fac} não existe!");
                return;
            }

            target.Faccao = fac;
            target.Rank = faccao.RankLider;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} te deu a liderança da facção {faccao.Nome} [{faccao.Codigo}].");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu a liderança da facção {faccao.Nome} [{faccao.Codigo}] para {target.Nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/lider {fac}", p, target);
        }

        [Command("parametros")]
        public void CMD_parametros(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Parâmetros do Servidor");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Recorde Online: {Global.Parametros.RecordeOnline}");
        }

        [Command("dinheiro")]
        public void CMD_dinheiro(Client player, string idNome, int dinheiro)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            target.Dinheiro += dinheiro;
            target.SetDinheiro();

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} te deu ${dinheiro.ToString("N0")}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu ${dinheiro.ToString("N0")} para {target.Nome}.");
            Functions.GravarLog(TipoLog.Staff, $"/dinheiro {dinheiro}", p, target);
        }

        [Command("cprop")]
        public void CMD_cprop(Client player, int interior, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), interior))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            var saida = Functions.ObterPosicaoPorInterior((TipoInterior)interior);

            var prop = new Entities.Propriedade()
            {
                EntradaPosX = player.Position.X,
                EntradaPosY = player.Position.Y,
                EntradaPosZ = player.Position.Z,
                Valor = valor,
                Personagem = 0,
                SaidaPosX = saida.X,
                SaidaPosY = saida.Y,
                SaidaPosZ = saida.Z,
            };

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Add(prop);
                context.SaveChanges();
            }

            prop.CriarIdentificador();

            Global.Propriedades.Add(prop);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} criada com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/cprop {prop.Codigo} {prop.Valor}", p, null);
        }

        [Command("rprop")]
        public void CMD_rprop(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe!");
                return;
            }

            if (prop.Personagem > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} possui um dono!");
                return;
            }

            using (var context = new RoleplayContext())
                context.Database.ExecuteSqlCommand($"DELETE FROM Propriedades WHERE Codigo = {codigo}");

            prop.DeletarIdentificador();

            Global.Propriedades.Remove(prop);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Propriedade {prop.Codigo} removida com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/rprop {prop.Codigo}", p, null);
        }

        [Command("epropvalor")]
        public void CMD_epropvalor(Client player, int codigo, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].Valor = valor;

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o valor da propriedade {prop.Codigo} para {valor}!");
            Functions.GravarLog(TipoLog.Staff, $"/epropvalor {prop.Codigo} {valor}", p, null);
        }

        [Command("epropint")]
        public void CMD_epropint(Client player, int codigo, int interior)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoInterior), interior))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Interior inválido!");
                return;
            }

            var pos = Functions.ObterPosicaoPorInterior((TipoInterior)interior);
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosX = pos.X;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosY = pos.Y;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].SaidaPosZ = pos.Z;

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o interior da propriedade {prop.Codigo} para {interior}!");
            Functions.GravarLog(TipoLog.Staff, $"/epropinterior {prop.Codigo} {interior}", p, null);
        }

        [Command("eproppos")]
        public void CMD_eproppos(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe!");
                return;
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosX = player.Position.X;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosY = player.Position.Y;
            Global.Propriedades[Global.Propriedades.IndexOf(prop)].EntradaPosZ = player.Position.Z;

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                context.SaveChanges();
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prop)].CriarIdentificador();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou a posição da propriedade {prop.Codigo} para sua posição atual (X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z})!");
            Functions.GravarLog(TipoLog.Staff, $"/eproppos {prop.Codigo} X: {player.Position.X} Y: {player.Position.Y} Z: {player.Position.Z}", p, null);
        }

        [Command("irprop")]
        public void CMD_irprop(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == codigo);
            if (prop == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Propriedade {codigo} não existe!");
                return;
            }

            player.Dimension = 0;
            player.Position = new Vector3(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você foi até a propriedade {prop.Codigo}!");
        }

        [Command("irblip")]
        public void CMD_irblip(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Codigo == codigo);
            if (blip == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Blip {codigo} não existe!");
                return;
            }

            player.Dimension = 0;
            player.Position = new Vector3(blip.PosX, blip.PosY, blip.PosZ);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você foi até o blip {blip.Codigo}!");
        }

        [Command("cpreco")]
        public void CMD_cpreco(Client player, int tipo, string nome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoPreco), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            if (nome.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 25 caracteres.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor deve ser maior que 0.");
                return;
            }

            if ((TipoPreco)tipo == TipoPreco.Veiculo)
            {
                var veh = NAPI.Util.VehicleNameToModel(nome);
                if ((int)veh == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não existe!");
                    return;
                }
                nome = veh.ToString();
            }

            using (var context = new RoleplayContext())
            {
                var preco = Global.Precos.FirstOrDefault(x => x.Tipo == tipo && x.Nome.ToLower() == nome.ToLower());
                if (preco == null)
                {
                    preco = new Entities.Preco()
                    {
                        Tipo = tipo,
                        Nome = nome,
                        Valor = valor,
                    };
                    Global.Precos.Add(preco);
                    context.Precos.Add(preco);
                }
                else
                {
                    Global.Precos[Global.Precos.IndexOf(preco)].Valor = valor;
                    context.Precos.Update(preco);
                }

                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {tipo} e nome {nome} criado/editado com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/cpreco {tipo} {nome} {valor}", p, null);
        }

        [Command("rpreco")]
        public void CMD_rpreco(Client player, int tipo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == tipo && x.Nome.ToLower() == nome.ToLower());
            if (preco == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Preço com tipo {tipo} e nome {nome} não existe!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                context.Precos.Remove(preco);
                context.SaveChanges();
            }

            Global.Precos.Remove(preco);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Preço com tipo {tipo} e nome {nome} removido com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/rpreco {tipo} {nome}", p, null);
        }

        [Command("cponto", GreedyArg = true)]
        public void CMD_cponto(Client player, int tipo, string nome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoPonto), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            if (nome.Length > 50)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                return;
            }

            var ponto = new Entities.Ponto()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Tipo = tipo,
                Nome = nome,
            };

            using (var context = new RoleplayContext())
            {
                context.Pontos.Add(ponto);
                context.SaveChanges();
            }

            ponto.CriarIdentificador();

            Global.Pontos.Add(ponto);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} criado com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/cponto {ponto.Codigo}", p, null);
        }

        [Command("rponto")]
        public void CMD_rponto(Client player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff < 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var ponto = Global.Pontos.FirstOrDefault(x => x.Codigo == codigo);
            if (ponto == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Ponto {codigo} não existe!");
                return;
            }
            using (var context = new RoleplayContext())
                context.Database.ExecuteSqlCommand($"DELETE FROM Pontos WHERE Codigo = {codigo}");

            ponto.DeletarIdentificador();

            Global.Pontos.Remove(ponto);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Ponto {ponto.Codigo} removido com sucesso!");
            Functions.GravarLog(TipoLog.Staff, $"/rponto {ponto.Codigo}", p, null);
        }
        #endregion Staff 1337
    }
}