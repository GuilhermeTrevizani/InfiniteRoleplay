using GTANetworkAPI;
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
                pl.Player.SendChatMessage("!{#32BBCE}" + $"(( [STAFF {p.UsuarioBD.Staff}] {p.UsuarioBD.Nome}: {mensagem} ))");
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
                pl.Player.SendChatMessage("!{#7FDDEB}" + $"(( {p.UsuarioBD.Nome}: {mensagem} ))");
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
                Functions.SalvarPersonagem(pl);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você salvou todos os personagens online.");
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
            float distanceVer = 10f;

            foreach (var b in Global.Blips)
            {
                float distance = player.Position.DistanceTo(new Vector3(b.PosX, b.PosY, b.PosZ));
                if (distance <= distanceVer)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"[BLIP] {b.Codigo} | {b.Nome}");
                    isTemAlgoProximo = true;
                }
            }

            if (!isTemAlgoProximo)
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum item!");
        }

        [Command("cblip", GreedyArg = true)]
        public void CMD_cblip(Client player, uint tipo, int cor, string nome = "")
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

            if (!string.IsNullOrWhiteSpace(nome))
            {
                if (nome.Length > 50)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                    return;
                }
            }

            var blip = new Entities.Blip()
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z,
                Cor = cor,
                Tipo = (int)tipo,
                Nome = nome,
            };

            using (var context = new RoleplayContext())
            {
                context.Blips.Add(blip);
                context.SaveChanges();
            }

            var blipv = NAPI.Blip.CreateBlip(player.Position);
            blipv.Sprite = tipo;
            blipv.Color = cor;
            blipv.Name = Functions.ObterNomePadraoBlip((int)tipo, nome);
            blipv.SetData(nameof(blip.Codigo), blip.Codigo);

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

            var blipv = NAPI.Pools.GetAllBlips().FirstOrDefault(x => x.GetData(nameof(blip.Codigo)) == codigo);
            blipv?.Delete();

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

        [Command("efac", GreedyArg = true)]
        public void CMD_efac(Client player, int codigo, string parametro, string valor)
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

            int.TryParse(valor, out int valorInt);

            switch (parametro)
            {
                case "nome":
                    if (valor.Length > 50)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome deve ter até 50 caracteres.");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Nome = valor;
                    break;
                case "abreviatura":
                    if (valor.Length > 10)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Abreviatura deve ter até 10 caracteres.");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Abreviatura = valor;
                    break;
                case "tipo":
                    if (!Enum.IsDefined(typeof(TipoFaccao), valorInt))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Tipo = valorInt;
                    break;
                case "cor":
                    if (valor.Length > 6)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cor deve ter até 6 caracteres.");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].Cor = valor;
                    break;
                case "rankgestor":
                    if (valorInt == 0)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank inválido!");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankGestor = valorInt;
                    break;
                case "ranklider":
                    if (valorInt == 0)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Rank inválido!");
                        return;
                    }

                    Global.Faccoes[Global.Faccoes.IndexOf(faccao)].RankLider = valorInt;
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Parâmetro inválido. Utilize: nome, abreviatura, tipo, cor, rankgestor, ranklider");
                    return;
            }

            using (var context = new RoleplayContext())
            {
                context.Faccoes.Update(Global.Faccoes[Global.Faccoes.IndexOf(faccao)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você editou o parâmetro {parametro} da facção {faccao.Codigo} para {valor}!");
            Functions.GravarLog(TipoLog.Staff, $"/efac {faccao.Codigo} {parametro} {valor}", p, null);
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
                player.SendChatMessage("!{#" + f.Cor + "}" + $"{f.Nome} [{f.Codigo}]");
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

            player.SendChatMessage("!{#" + faction.Cor + "}" + $"{faction.Nome} [{faction.Codigo}]");
            foreach (var r in ranks)
                player.SendChatMessage($"{r.Nome} [{r.Codigo}]");
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
        #endregion Staff 1337
    }
}