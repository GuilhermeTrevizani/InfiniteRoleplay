using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using InfiniteRoleplay.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InfiniteRoleplay
{
    public static class Functions
    {
        public static void EnviarMensagem(Player player, TipoMensagem tipoMensagem, string mensagem)
        {
            var strTipo = string.Empty;
            if (tipoMensagem == TipoMensagem.Sucesso)
                strTipo = "!{#6EB469}";
            else if (tipoMensagem == TipoMensagem.Erro)
                strTipo = "!{#FF6A4D}";
            else if (tipoMensagem == TipoMensagem.Titulo)
                strTipo = "!{#B0B0B0}";
            else if (tipoMensagem == TipoMensagem.Punicao)
                strTipo = "!{#FF6A4D}";

            player.SendChatMessage($"{strTipo}{mensagem}");
        }

        public static string Criptografar(string texto)
        {
            var encodedValue = Encoding.UTF8.GetBytes(texto);
            var encryptedPassword = SHA512.Create().ComputeHash(encodedValue);

            var sb = new StringBuilder();
            foreach (var caracter in encryptedPassword)
                sb.Append(caracter.ToString("X2"));

            return sb.ToString();
        }

        public static void LogarPersonagem(Player player, Personagem p)
        {
            p.Convites = new List<Convite>();
            player.Name = $"{p.Nome} [{p.ID}]";
            player.Dimension = (uint)p.Dimensao;
            p.IPLs = JsonConvert.DeserializeObject<List<string>>(p.IPL);
            foreach (var ipl in p.IPLs)
                NAPI.ClientEvent.TriggerClientEvent(player, "setIPL", ipl);
            player.Position = new Vector3(p.PosX, p.PosY, p.PosZ);
            player.Rotation = new Vector3(p.RotX, p.RotY, p.RotZ);
            player.Health = p.Vida;
            player.Armor = p.Colete;
            player.SetSkin(NAPI.Util.PedNameToModel(p.Skin));
            p.SetDinheiro();

            using (var context = new RoleplayContext())
            {
                p.Contatos = context.PersonagensContatos.Where(x => x.Codigo == p.Codigo).ToList();

                if (Global.PersonagensOnline.Count > Global.Parametros.RecordeOnline)
                {
                    Global.Parametros.RecordeOnline = Global.PersonagensOnline.Count;
                    context.Parametros.Update(Global.Parametros);
                    context.SaveChanges();
                }
            }

            GravarLog(TipoLog.Entrada, string.Empty, p, null);

            NAPI.ClientEvent.TriggerClientEvent(player, "logarPersonagem");
        }

        public static Personagem ObterPersonagem(Player player)
        {
            return Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == player?.SocialClubName);
        }

        public static Personagem ObterPersonagemPorIdNome(Player player, string idNome, bool isPodeProprioPlayer = true)
        {
            int.TryParse(idNome, out int id);
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.ID == id);
            if (p != null)
            {
                if (!isPodeProprioPlayer && player == p.Player)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você!");
                    return null;
                }

                return p;
            }

            var ps = Global.PersonagensOnline.Where(x => x.Nome.ToLower().Contains(idNome.Replace("_", " ").ToLower())).ToList();
            if (ps.Count == 1)
            {
                if (!isPodeProprioPlayer && player == ps.FirstOrDefault().Player)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, $"O jogador não pode ser você!");
                    return null;
                }

                return ps.FirstOrDefault();
            }

            if (ps.Count > 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Mais de um jogador foi encontrado com a pesquisa: {idNome}");
                foreach (var pl in ps)
                    EnviarMensagem(player, TipoMensagem.Nenhum, $"[ID: {pl.ID}] {pl.Nome}");
            }
            else
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum jogador foi encontrado com a pesquisa: {idNome}");
            }

            return null;
        }

        public static void SalvarPersonagem(Personagem p, bool isOnline = true)
        {
            var dif = DateTime.Now - p.DataUltimaVerificacao;
            if (dif.TotalMinutes >= 1)
            {
                p.TempoConectado++;
                p.DataUltimaVerificacao = DateTime.Now;

                if (p.TempoPrisao > 0)
                {
                    p.TempoPrisao--;
                    if (p.TempoPrisao == 0)
                    {
                        p.Player.Position = new Vector3(432.8367, -981.7594, 30.71048);
                        p.Player.Rotation = new Vector3(0, 0, 86.37479);
                        EnviarMensagem(p.Player, TipoMensagem.Sucesso, $"Seu tempo de prisão acabou e você foi libertado!");
                    }
                }

                if (p.TempoConectado % 60 == 0)
                {
                    var salario = 0;
                    if (p.Faccao > 0)
                        salario += p.RankBD.Salario;
                    else if (p.Emprego > 0)
                        salario += Global.Parametros.ValorIncentivoGovernamental;

                    if (Convert.ToInt32(p.TempoConectado / 60) <= Global.Parametros.HorasIncentivoInicial)
                        salario += Global.Parametros.ValorIncentivoInicial;

                    p.Banco += salario;
                    if (salario > 0)
                        EnviarMensagem(p.Player, TipoMensagem.Sucesso, $"Seu salário de ${salario:N0} foi depositado no banco!");
                }

                if (p.IsEmTrabalhoAdministrativo)
                    p.UsuarioBD.TempoTrabalhoAdministrativo++;
            }

            if (!isOnline && p.Celular > 0)
            {
                p.LimparLigacao();
                var pLigando = Global.PersonagensOnline.FirstOrDefault(x => x.NumeroLigacao == p.Celular);
                if (pLigando != null)
                {
                    pLigando.LimparLigacao();
                    EnviarMensagem(pLigando.Player, TipoMensagem.Nenhum, "!{#e6a250}" + $"[CELULAR] Sua ligação para {pLigando.ObterNomeContato(p.Celular)} caiu!");
                }
            }

            using var context = new RoleplayContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == p.Codigo);
            personagem.Online = isOnline;
            personagem.Skin = ((PedHash)p.Player.Model).ToString();
            personagem.PosX = p.Player.Position.X;
            personagem.PosY = p.Player.Position.Y;
            personagem.PosZ = p.Player.Position.Z;
            personagem.Vida = p.Player.Health;
            personagem.Colete = p.Player.Armor;
            personagem.Dimensao = p.Player.Dimension;
            personagem.TempoConectado = p.TempoConectado;
            personagem.Faccao = p.Faccao;
            personagem.Rank = p.Rank;
            personagem.Dinheiro = p.Dinheiro;
            personagem.Celular = p.Celular;
            personagem.Banco = p.Banco;
            personagem.IPL = JsonConvert.SerializeObject(p.IPLs);
            personagem.CanalRadio = p.CanalRadio;
            personagem.CanalRadio2 = p.CanalRadio2;
            personagem.CanalRadio3 = p.CanalRadio3;
            personagem.TempoPrisao = p.TempoPrisao;
            personagem.RotX = p.Player.Rotation.X;
            personagem.RotY = p.Player.Rotation.Y;
            personagem.RotZ = p.Player.Rotation.Z;
            personagem.DataMorte = p.DataMorte;
            personagem.MotivoMorte = p.MotivoMorte;
            personagem.Emprego = p.Emprego;
            personagem.DataUltimoAcesso = DateTime.Now;
            personagem.IPUltimoAcesso = p.Player.Address;
            context.Personagens.Update(personagem);

            context.Database.ExecuteSqlRaw($"DELETE FROM PersonagensContatos WHERE Codigo = {p.Codigo}");
            context.PersonagensContatos.AddRange(p.Contatos);

            var usuario = context.Usuarios.FirstOrDefault(x => x.Codigo == p.UsuarioBD.Codigo);
            usuario.Staff = p.UsuarioBD.Staff;
            usuario.TempoTrabalhoAdministrativo = p.UsuarioBD.TempoTrabalhoAdministrativo;
            usuario.QuantidadeSOSAceitos = p.UsuarioBD.QuantidadeSOSAceitos;
            usuario.DataUltimoAcesso = DateTime.Now;
            usuario.IPUltimoAcesso = p.Player.Address;
            context.Usuarios.Update(usuario);

            context.SaveChanges();
        }

        public static void SendMessageToNearbyPlayers(Player player, string message, TipoMensagemJogo type, float range, bool excludePlayer = false)
        {
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
            var distanceGap = range / 5;

            var targetList = NAPI.Pools.GetAllPlayers().Where(x => Global.PersonagensOnline.Any(y => y.UsuarioBD.SocialClubRegistro == x.SocialClubName) && x.Dimension == player.Dimension).ToList();

            foreach (var target in targetList)
            {
                if (player != target || (player == target && !excludePlayer))
                {
                    var distance = player.Position.DistanceTo(target.Position);

                    if (distance <= range)
                    {
                        var chatMessageColor = GetChatMessageColor(distance, distanceGap);
                        switch (type)
                        {
                            case TipoMensagemJogo.ChatICNormal:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{p.NomeIC} diz: {message}");
                                break;
                            case TipoMensagemJogo.ChatICGrito:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{p.NomeIC} grita: {message}");
                                break;
                            case TipoMensagemJogo.Me:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#C2A2DA}" + $"* {p.NomeIC} {message}");
                                break;
                            case TipoMensagemJogo.Do:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#C2A2DA}" + $"* {message} (( {p.NomeIC} ))");
                                break;
                            case TipoMensagemJogo.ChatOOC:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#bababa}" + $"(( {p.NomeIC} [{p.ID}]: {message} ))");
                                break;
                            case TipoMensagemJogo.ChatICBaixo:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{p.NomeIC} diz [baixo]: {message}");
                                break;
                            case TipoMensagemJogo.Megafone:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#F2FF43}" + $"{p.NomeIC} diz [megafone]: {message}");
                                break;
                            case TipoMensagemJogo.Celular:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{p.NomeIC} [celular]: {message}");
                                break;
                            case TipoMensagemJogo.Ame:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#C2A2DA}" + $"* {p.NomeIC} {message}");
                                break;
                            case TipoMensagemJogo.Radio:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{p.NomeIC} [rádio]: {message}");
                                break;
                        }
                    }
                }
            }
        }

        private static string GetChatMessageColor(float distance, float distanceGap)
        {
            if (distance < distanceGap)
                return "!{#E6E6E6}";
            else if (distance < distanceGap * 2)
                return "!{#C8C8C8}";
            else if (distance < distanceGap * 3)
                return "!{#AAAAAA}";
            else if (distance < distanceGap * 4)
                return "!{#8C8C8C}";

            return "!{#6E6E6E}";
        }

        public static void MostrarStats(Player player, Personagem p)
        {
            EnviarMensagem(player, TipoMensagem.Titulo, $"Informações de {p.Nome} [{p.Codigo}]");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"OOC: {p.UsuarioBD.Nome} | SocialClub: {p.Player.SocialClubName} | Registro: {p.DataRegistro}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Tempo Conectado (minutos): {p.TempoConectado} | Celular: {p.Celular} | Emprego: {ObterDisplayEnum((TipoEmprego)p.Emprego)}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Sexo: {p.Sexo} | Nascimento: {p.DataNascimento.ToShortDateString()} | Dinheiro: ${p.Dinheiro:N0} | Banco: ${p.Banco:N0}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Skin: {(PedHash)p.Player.Model} | Vida: {p.Player.Health} | Colete: {p.Player.Armor} | Tempo de Prisão: {p.TempoPrisao}");

            if (p.UsuarioBD.Staff > 0)
                EnviarMensagem(player, TipoMensagem.Nenhum, $"Staff: {p.UsuarioBD.NomeStaff} [{p.UsuarioBD.Staff}] | Tempo Serviço Administrativo (minutos): {p.UsuarioBD.TempoTrabalhoAdministrativo} | SOSs Aceitos: {p.UsuarioBD.QuantidadeSOSAceitos}");

            if (p.CanalRadio > -1)
                EnviarMensagem(player, TipoMensagem.Nenhum, $"Canal Rádio 1: {p.CanalRadio} | Canal Rádio 2: {p.CanalRadio2} | Canal Rádio 3: {p.CanalRadio3}");

            if (p.Faccao > 0)
                EnviarMensagem(player, TipoMensagem.Nenhum, $"Facção: {p.FaccaoBD.Nome} [{p.Faccao}] | Rank: {p.RankBD.Nome} [{p.Rank}] | Salário: ${p.RankBD.Salario:N0}");

            if (p.Propriedades.Count > 0)
            {
                EnviarMensagem(player, TipoMensagem.Titulo, $"Propriedades de {p.Nome} [{p.Codigo}]");
                foreach (var prop in p.Propriedades)
                    EnviarMensagem(player, TipoMensagem.Nenhum, $"Código: {prop.Codigo} | Valor: ${prop.Valor:N0}");
            }
        }

        public static bool VerificarBanimento(Player player, Banimento ban)
        {
            if (ban == null)
                return true;

            using (var context = new RoleplayContext())
            {
                if (ban.Expiracao != null)
                {
                    if (DateTime.Now > ban.Expiracao)
                    {
                        context.Banimentos.Remove(ban);
                        context.SaveChanges();
                        return true;
                    }
                }

                var staff = context.Usuarios.FirstOrDefault(x => x.Codigo == ban.UsuarioStaff)?.Nome;
                var strBan = ban.Expiracao == null ? " permanentemente." : $". Seu banimento expira em: {ban.Expiracao?.ToString()}";

                NAPI.ClientEvent.TriggerClientEvent(player, "mensagem", $"Você está banido{strBan}<br/>Data: {ban.Data} | Motivo: {ban.Motivo} | Staff: {staff}");
            }

            player.Kick();
            return false;
        }

        public static void GravarLog(TipoLog tipo, string descricao, Personagem origem, Personagem destino)
        {
            using var context = new RoleplayContext();
            context.Logs.Add(new Log()
            {
                Data = DateTime.Now,
                Tipo = (int)tipo,
                Descricao = descricao,
                PersonagemOrigem = origem.Codigo,
                IPOrigem = origem.Player.Address,
                SocialClubOrigem = origem.Player.SocialClubName,
                PersonagemDestino = destino?.Codigo ?? 0,
                IPDestino = destino?.Player.Address ?? string.Empty,
                SocialClubDestino = destino?.Player?.SocialClubName ?? string.Empty,
            });
            context.SaveChanges();
        }

        public static int ObterNovoID()
        {
            for (var i = 1; i <= NAPI.Server.GetMaxPlayers(); i++)
            {
                if (Global.PersonagensOnline.Any(x => x.ID == i))
                    continue;

                return i;
            }

            return 1;
        }

        public static Vector3 ObterPosicaoPorInterior(TipoInterior tipo)
        {
            return tipo switch
            {
                TipoInterior.Motel => new Vector3(151.2564, -1007.868, -98.99999),
                TipoInterior.CasaBaixa => new Vector3(265.9522, -1007.485, -101.0085),
                TipoInterior.CasaMedia => new Vector3(346.4499, -1012.996, -99.19622),
                TipoInterior.IntegrityWay28 => new Vector3(-31.34092, -594.9429, 80.0309),
                TipoInterior.IntegrityWay30 => new Vector3(-17.61359, -589.3938, 90.11487),
                TipoInterior.DellPerroHeights4 => new Vector3(-1452.225, -540.4642, 74.04436),
                TipoInterior.DellPerroHeights7 => new Vector3(-1451.26, -523.9634, 56.92898),
                TipoInterior.RichardMajestic2 => new Vector3(-912.6351, -364.9724, 114.2748),
                TipoInterior.TinselTowers42 => new Vector3(-603.1113, 58.93406, 98.20017),
                TipoInterior.EclipseTowers3 => new Vector3(-785.1537, 323.8156, 211.9973),
                TipoInterior.WildOatsDrive3655 => new Vector3(-174.3753, 497.3086, 137.6669),
                TipoInterior.NorthConkerAvenue2044 => new Vector3(341.9306, 437.7751, 149.3901),
                TipoInterior.NorthConkerAvenue2045 => new Vector3(373.5803, 423.7043, 145.9078),
                TipoInterior.HillcrestAvenue2862 => new Vector3(-682.3693, 592.2678, 145.393),
                TipoInterior.HillcrestAvenue2868 => new Vector3(-758.4348, 618.8454, 144.1539),
                TipoInterior.HillcrestAvenue2874 => new Vector3(-859.7643, 690.8358, 152.8607),
                TipoInterior.WhispymoundDrive2677 => new Vector3(117.209, 559.8086, 184.3048),
                TipoInterior.MadWayneThunder2133 => new Vector3(-1289.775, 449.3125, 97.90256),
                TipoInterior.Modern1Apartment => new Vector3(-786.8663, 315.7642, 217.6385),
                TipoInterior.Modern2Apartment => new Vector3(-786.9563, 315.6229, 187.9136),
                TipoInterior.Modern3Apartment => new Vector3(-774.0126, 342.0428, 196.6864),
                TipoInterior.Mody1Apartment => new Vector3(-787.0749, 315.8198, 217.6386),
                TipoInterior.Mody2Apartment => new Vector3(-786.8195, 315.5634, 187.9137),
                TipoInterior.Mody3Apartment => new Vector3(-774.1382, 342.0316, 196.6864),
                TipoInterior.Vibrant1Apartment => new Vector3(-786.6245, 315.6175, 217.6385),
                TipoInterior.Vibrant2Apartment => new Vector3(-786.9584, 315.7974, 187.9135),
                TipoInterior.Vibrant3Apartment => new Vector3(-774.0223, 342.1718, 196.6863),
                TipoInterior.Sharp1Apartment => new Vector3(-787.0902, 315.7039, 217.6384),
                TipoInterior.Sharp2Apartment => new Vector3(-787.0155, 315.7071, 187.9135),
                TipoInterior.Sharp3Apartment => new Vector3(-773.8976, 342.1525, 196.6863),
                TipoInterior.Monochrome1Apartment => new Vector3(-786.9887, 315.7393, 217.6386),
                TipoInterior.Monochrome2Apartment => new Vector3(-786.8809, 315.6634, 187.9136),
                TipoInterior.Monochrome3Apartment => new Vector3(-774.0675, 342.0773, 196.6864),
                TipoInterior.Seductive1Apartment => new Vector3(-787.1423, 315.6943, 217.6384),
                TipoInterior.Seductive2Apartment => new Vector3(-787.0961, 315.815, 187.9135),
                TipoInterior.Seductive3Apartment => new Vector3(-773.9552, 341.9892, 196.6862),
                TipoInterior.Regal1Apartment => new Vector3(-787.029, 315.7113, 217.6385),
                TipoInterior.Regal2Apartment => new Vector3(-787.0574, 315.6567, 187.9135),
                TipoInterior.Regal3Apartment => new Vector3(-774.0109, 342.0965, 196.6863),
                TipoInterior.Aqua1Apartment => new Vector3(-786.9469, 315.5655, 217.6383),
                TipoInterior.Aqua2Apartment => new Vector3(-786.9756, 315.723, 187.9134),
                TipoInterior.Aqua3Apartment => new Vector3(-774.0349, 342.0296, 196.6862),
                TipoInterior.ArcadiusExecutiveRich => new Vector3(-141.1987, -620.913, 168.8205),
                TipoInterior.ArcadiusExecutiveCool => new Vector3(-141.5429, -620.9524, 168.8204),
                TipoInterior.ArcadiusExecutiveContrast => new Vector3(-141.2896, -620.9618, 168.8204),
                TipoInterior.ArcadiusOldSpiceWarm => new Vector3(-141.4966, -620.8292, 168.8204),
                TipoInterior.ArcadiusOldSpiceClassical => new Vector3(-141.3997, -620.9006, 168.8204),
                TipoInterior.ArcadiusOldSpiceVintage => new Vector3(-141.5361, -620.9186, 168.8204),
                TipoInterior.ArcadiusPowerBrokerIce => new Vector3(-141.392, -621.0451, 168.8204),
                TipoInterior.ArcadiusPowerBrokeConservative => new Vector3(-141.1945, -620.8729, 168.8204),
                TipoInterior.ArcadiusPowerBrokePolished => new Vector3(-141.4924, -621.0035, 168.8205),
                TipoInterior.MazeBankExecutiveRich => new Vector3(-75.8466, -826.9893, 243.3859),
                TipoInterior.MazeBankExecutiveCool => new Vector3(-75.49945, -827.05, 243.386),
                TipoInterior.MazeBankExecutiveContrast => new Vector3(-75.49827, -827.1889, 243.386),
                TipoInterior.MazeBankOldSpiceWarm => new Vector3(-75.44054, -827.1487, 243.3859),
                TipoInterior.MazeBankOldSpiceClassical => new Vector3(-75.63942, -827.1022, 243.3859),
                TipoInterior.MazeBankOldSpiceVintage => new Vector3(-75.47446, -827.2621, 243.386),
                TipoInterior.MazeBankPowerBrokerIce => new Vector3(-75.56978, -827.1152, 243.3859),
                TipoInterior.MazeBankPowerBrokeConservative => new Vector3(-75.51953, -827.0786, 243.3859),
                TipoInterior.MazeBankPowerBrokePolished => new Vector3(-75.41915, -827.1118, 243.3858),
                TipoInterior.LomBankExecutiveRich => new Vector3(-1579.756, -565.0661, 108.523),
                TipoInterior.LomBankExecutiveCool => new Vector3(-1579.678, -565.0034, 108.5229),
                TipoInterior.LomBankExecutiveContrast => new Vector3(-1579.583, -565.0399, 108.5229),
                TipoInterior.LomBankOldSpiceWarm => new Vector3(-1579.702, -565.0366, 108.5229),
                TipoInterior.LomBankOldSpiceClassical => new Vector3(-1579.643, -564.9685, 108.5229),
                TipoInterior.LomBankOldSpiceVintage => new Vector3(-1579.681, -565.0003, 108.523),
                TipoInterior.LomBankPowerBrokerIce => new Vector3(-1579.677, -565.0689, 108.5229),
                TipoInterior.LomBankPowerBrokeConservative => new Vector3(-1579.708, -564.9634, 108.5229),
                TipoInterior.LomBankPowerBrokePolished => new Vector3(-1579.693, -564.8981, 108.5229),
                TipoInterior.MazeBankWestExecutiveRich => new Vector3(-1392.667, -480.4736, 72.04217),
                TipoInterior.MazeBankWestExecutiveCool => new Vector3(-1392.542, -480.4011, 72.04211),
                TipoInterior.MazeBankWestExecutiveContrast => new Vector3(-1392.626, -480.4856, 72.04212),
                TipoInterior.MazeBankWestOldSpiceWarm => new Vector3(-1392.617, -480.6363, 72.04208),
                TipoInterior.MazeBankWestOldSpiceClassical => new Vector3(-1392.532, -480.7649, 72.04207),
                TipoInterior.MazeBankWestOldSpiceVintage => new Vector3(-1392.611, -480.5562, 72.04214),
                TipoInterior.MazeBankWestPowerBrokerIce => new Vector3(-1392.563, -480.549, 72.0421),
                TipoInterior.MazeBankWestPowerBrokeConservative => new Vector3(-1392.528, -480.475, 72.04206),
                TipoInterior.MazeBankWestPowerBrokePolished => new Vector3(-1392.416, -480.7485, 72.04207),
                _ => new Vector3(),
            };
        }

        public static void EnviarMensagemCelular(Personagem p, Personagem target, string mensagem)
        {
            SendMessageToNearbyPlayers(p.Player, mensagem, TipoMensagemJogo.Celular, p.Player.Dimension > 0 ? 7.5f : 10.0f, false);

            if (target != null)
                EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {target.ObterNomeContato(p.Celular)} diz: {mensagem}");
        }

        public static void EnviarMensagemTipoFaccao(TipoFaccao tipo, string mensagem, bool isSomenteParaTrabalho, bool isCorFaccao)
        {
            var players = Global.PersonagensOnline.Where(x => x.FaccaoBD?.Tipo == (int)tipo);

            if (isSomenteParaTrabalho)
                players = players.Where(x => x.IsEmTrabalho);

            foreach (var pl in players)
                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, (isCorFaccao ? "!{#" + pl.FaccaoBD.Cor + "}" : string.Empty) + mensagem);
        }

        public static void ComprarVeiculo(Player player, int tipo, string erro)
        {
            if (tipo == 0)
            {
                foreach (var c in Global.Concessionarias)
                {
                    if (tipo == 0 && player.Position.DistanceTo(c.PosicaoCompra) <= 2)
                        tipo = (int)c.Tipo;
                }

                if (tipo == 0)
                {
                    EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma concessionária!");
                    return;
                }
            }

            var veiculos = Global.Precos.Where(x => x.Tipo == tipo).OrderBy(x => x.Nome).Select(x => new
            {
                x.Nome,
                Preco = $"${x.Valor:N0}",
            }).ToList();

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoVComprar", tipo, veiculos, erro);
        }

        public static string GerarPlacaVeiculo()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return $"{chars[random.Next(25)]}{chars[random.Next(25)]}{random.Next(0, 99999).ToString().PadLeft(5, '0')}{chars[random.Next(25)]}";
        }

        public static void AbrirCelular(Player player, string msg, int tipoMsg)
        {
            var p = ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            if (p.TempoPrisao > 0 || p.Algemado)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode usar o celular agora!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "abrirCelular", p.Contatos.OrderBy(x => x.Nome).ToList(), msg, tipoMsg);
        }

        public static void VisualizarMultas(Player player, string erro)
        {
            var p = ObterPersonagem(player);
            if (p == null)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == (int)TipoPonto.Multas && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de pagamento de multas!");
                return;
            }

            using var context = new RoleplayContext();
            var multas = context.Multas.Where(x => !x.DataPagamento.HasValue && x.PersonagemMultado == p.Codigo).OrderBy(x => x.Data).Select(x => new
            {
                x.Codigo,
                x.Motivo,
                Data = x.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                Valor = $"${x.Valor:N0}",
            }).ToList();
            if (multas.Count == 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não possui multas pendentes!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "visualizarMultas", multas, erro);
        }

        public static List<string> ObterIPLsPorInterior(TipoInterior tipo)
        {
            return tipo switch
            {
                TipoInterior.Modern1Apartment => new List<string> { "apa_v_mp_h_01_a" },
                TipoInterior.Modern2Apartment => new List<string> { "apa_v_mp_h_01_c" },
                TipoInterior.Modern3Apartment => new List<string> { "apa_v_mp_h_01_b" },
                TipoInterior.Mody1Apartment => new List<string> { "apa_v_mp_h_02_a" },
                TipoInterior.Mody2Apartment => new List<string> { "apa_v_mp_h_02_c" },
                TipoInterior.Mody3Apartment => new List<string> { "apa_v_mp_h_02_b" },
                TipoInterior.Vibrant1Apartment => new List<string> { "apa_v_mp_h_03_a" },
                TipoInterior.Vibrant2Apartment => new List<string> { "apa_v_mp_h_03_c" },
                TipoInterior.Vibrant3Apartment => new List<string> { "apa_v_mp_h_03_b" },
                TipoInterior.Sharp1Apartment => new List<string> { "apa_v_mp_h_04_a" },
                TipoInterior.Sharp2Apartment => new List<string> { "apa_v_mp_h_04_c" },
                TipoInterior.Sharp3Apartment => new List<string> { "apa_v_mp_h_04_b" },
                TipoInterior.Monochrome1Apartment => new List<string> { "apa_v_mp_h_05_a" },
                TipoInterior.Monochrome2Apartment => new List<string> { "apa_v_mp_h_05_c" },
                TipoInterior.Monochrome3Apartment => new List<string> { "apa_v_mp_h_05_b" },
                TipoInterior.Seductive1Apartment => new List<string> { "apa_v_mp_h_06_a" },
                TipoInterior.Seductive2Apartment => new List<string> { "apa_v_mp_h_06_c" },
                TipoInterior.Seductive3Apartment => new List<string> { "apa_v_mp_h_06_b" },
                TipoInterior.Regal1Apartment => new List<string> { "apa_v_mp_h_07_a" },
                TipoInterior.Regal2Apartment => new List<string> { "apa_v_mp_h_07_c" },
                TipoInterior.Regal3Apartment => new List<string> { "apa_v_mp_h_07_b" },
                TipoInterior.Aqua1Apartment => new List<string> { "apa_v_mp_h_08_a" },
                TipoInterior.Aqua2Apartment => new List<string> { "apa_v_mp_h_08_c" },
                TipoInterior.Aqua3Apartment => new List<string> { "apa_v_mp_h_08_b" },
                TipoInterior.ArcadiusExecutiveRich => new List<string> { "ex_dt1_02_office_02b" },
                TipoInterior.ArcadiusExecutiveCool => new List<string> { "ex_dt1_02_office_02c" },
                TipoInterior.ArcadiusExecutiveContrast => new List<string> { "ex_dt1_02_office_02a" },
                TipoInterior.ArcadiusOldSpiceWarm => new List<string> { "ex_dt1_02_office_01a" },
                TipoInterior.ArcadiusOldSpiceClassical => new List<string> { "ex_dt1_02_office_01b" },
                TipoInterior.ArcadiusOldSpiceVintage => new List<string> { "ex_dt1_02_office_01c" },
                TipoInterior.ArcadiusPowerBrokerIce => new List<string> { "ex_dt1_02_office_03a" },
                TipoInterior.ArcadiusPowerBrokeConservative => new List<string> { "ex_dt1_02_office_03b" },
                TipoInterior.ArcadiusPowerBrokePolished => new List<string> { "ex_dt1_02_office_03c" },
                TipoInterior.MazeBankExecutiveRich => new List<string> { "ex_dt1_11_office_02b" },
                TipoInterior.MazeBankExecutiveCool => new List<string> { "ex_dt1_11_office_02c" },
                TipoInterior.MazeBankExecutiveContrast => new List<string> { "ex_dt1_11_office_02a" },
                TipoInterior.MazeBankOldSpiceWarm => new List<string> { "ex_dt1_11_office_01a" },
                TipoInterior.MazeBankOldSpiceClassical => new List<string> { "ex_dt1_11_office_01b" },
                TipoInterior.MazeBankOldSpiceVintage => new List<string> { "ex_dt1_11_office_01c" },
                TipoInterior.MazeBankPowerBrokerIce => new List<string> { "ex_dt1_11_office_03a" },
                TipoInterior.MazeBankPowerBrokeConservative => new List<string> { "ex_dt1_11_office_03b" },
                TipoInterior.MazeBankPowerBrokePolished => new List<string> { "ex_dt1_11_office_03c" },
                TipoInterior.LomBankExecutiveRich => new List<string> { "ex_sm_13_office_02b" },
                TipoInterior.LomBankExecutiveCool => new List<string> { "ex_sm_13_office_02c" },
                TipoInterior.LomBankExecutiveContrast => new List<string> { "ex_sm_13_office_02a" },
                TipoInterior.LomBankOldSpiceWarm => new List<string> { "ex_sm_13_office_01a" },
                TipoInterior.LomBankOldSpiceClassical => new List<string> { "ex_sm_13_office_01b" },
                TipoInterior.LomBankOldSpiceVintage => new List<string> { "ex_sm_13_office_01c" },
                TipoInterior.LomBankPowerBrokerIce => new List<string> { "ex_sm_13_office_03a" },
                TipoInterior.LomBankPowerBrokeConservative => new List<string> { "ex_sm_13_office_03b" },
                TipoInterior.LomBankPowerBrokePolished => new List<string> { "ex_sm_13_office_03c" },
                TipoInterior.MazeBankWestExecutiveRich => new List<string> { "ex_sm_15_office_02b" },
                TipoInterior.MazeBankWestExecutiveCool => new List<string> { "ex_sm_15_office_02c" },
                TipoInterior.MazeBankWestExecutiveContrast => new List<string> { "ex_sm_15_office_02a" },
                TipoInterior.MazeBankWestOldSpiceWarm => new List<string> { "ex_sm_15_office_01a" },
                TipoInterior.MazeBankWestOldSpiceClassical => new List<string> { "ex_sm_15_office_01b" },
                TipoInterior.MazeBankWestOldSpiceVintage => new List<string> { "ex_sm_15_office_01c" },
                TipoInterior.MazeBankWestPowerBrokerIce => new List<string> { "ex_sm_15_office_03a" },
                TipoInterior.MazeBankWestPowerBrokeConservative => new List<string> { "ex_sm_15_office_03b" },
                TipoInterior.MazeBankWestPowerBrokePolished => new List<string> { "ex_sm_15_office_03c" },
                _ => new List<string>(),
            };
        }

        public static bool ChecarAnimacoes(Player player, bool isPararAnim = false)
        {
            if (player.IsInVehicle)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação em um veículo!");
                return false;
            }

            var p = ObterPersonagem(player);
            if (p.Algemado)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação algemado!");
                return false;
            }

            // Verificar aqui status de ferido, impossibilitado de fazer alguma animação

            player.StopAnimation();
            player.SetSharedData("IsAnimacao", !isPararAnim);
            return true;
        }

        public static void EnviarMensagemChat(Personagem p, string message, TipoMensagemJogo tipoMensagemJogo)
        {
            if (string.IsNullOrWhiteSpace(message) || p == null)
                return;

            if (p.StatusLigacao > 0)
            {
                EnviarMensagemCelular(p, Global.PersonagensOnline.FirstOrDefault(x => x.Celular == p.NumeroLigacao), message);

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
                            EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: Não entendi sua mensagem. Deseja falar com LSPD ou LSFD?");
                            return;
                        }

                        p.StatusLigacao = 2;
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: {p.ExtraLigacao}, qual sua emergência?");
                        return;
                    }

                    if (p.StatusLigacao == 2)
                    {
                        EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(911)} diz: Nossas unidades foram alertadas!");

                        var tipoFaccao = p.ExtraLigacao == "LSPD" ? TipoFaccao.Policial : TipoFaccao.Medica;
                        EnviarMensagemTipoFaccao(tipoFaccao, $"Central de Emergência | Ligação 911", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"De: ~w~{p.Celular}", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"Mensagem: ~w~{message}", true, true);

                        p.LimparLigacao();
                    }
                }
                else if (p.NumeroLigacao == 5555555)
                {
                    EnviarMensagem(p.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] {p.ObterNomeContato(5555555)} diz: Nossos taxistas em serviço foram alertados e você receberá um SMS de confirmação!");

                    p.AguardandoTipoServico = (int)TipoEmprego.Taxista;

                    EnviarMensagemEmprego(TipoEmprego.Taxista, "!{#fcba03}" + $"Downtown Cab Company | Solicitação de Táxi #{p.Codigo}");
                    EnviarMensagemEmprego(TipoEmprego.Taxista, "!{#fcba03}" + $"De: ~w~{p.Celular}");
                    EnviarMensagemEmprego(TipoEmprego.Taxista, "!{#fcba03}" + $"Destino: ~w~{message}");

                    p.LimparLigacao();
                }

                return;
            }

            var targetLigacao = Global.PersonagensOnline.FirstOrDefault(x => x.StatusLigacao > 0 && x.NumeroLigacao == p.Celular);
            if (targetLigacao != null)
            {
                EnviarMensagemCelular(p, targetLigacao, message);
                return;
            }

            SendMessageToNearbyPlayers(p.Player, message, tipoMensagemJogo, p.Player.Dimension > 0 ? 7.5f : 10.0f);
        }

        public static void EnviarMensagemRadio(Personagem p, int slot, string mensagem)
        {
            if (p == null)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.CanalRadio == -1)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você não possui um rádio!");
                return;
            }

            if (p.TempoPrisao > 0)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, "Você está preso!");
                return;
            }

            var canal = p.CanalRadio;
            if (slot == 2)
                canal = p.CanalRadio2;
            else if (slot == 3)
                canal = p.CanalRadio3;

            if (canal == 0)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Seu slot {slot} do rádio não possui um canal configurado!");
                return;
            }

            if ((canal == 911 || canal == 912) && !p.IsEmTrabalho)
            {
                EnviarMensagem(p.Player, TipoMensagem.Erro, $"Você só pode falar no canal {canal} quando estiver em serviço!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.CanalRadio == canal || x.CanalRadio2 == canal || x.CanalRadio3 == canal);
            foreach (var pl in players)
            {
                var slotPl = 1;
                if (pl.CanalRadio2 == canal)
                    slotPl = 2;
                else if (pl.CanalRadio3 == canal)
                    slotPl = 3;

                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#FFFF9B}" + $"[S:{slotPl} C:{canal}] {p.Nome}: {mensagem}");
            }

            EnviarMensagemChat(p, mensagem, TipoMensagemJogo.Radio);
        }

        public static void CarregarSkins()
        {
            Global.Skins = new List<Skin>()
            {
                new Skin("Rat", true),
                new Skin("Corpse01",true),
                new Skin("Corpse02", true),
                new Skin("MovAlien01", true),
                new Skin("MovSpace01SMM",true),
                new Skin("Orleans", true),
                new Skin("PestContGunman", true),
                new Skin("Pogo01", true),
                new Skin("PrisMuscl01SMY", true),
                new Skin("Prisoner01", true),
                new Skin("Prisoner01SMY", true),
                new Skin("RashKovsky", true),
                new Skin("RsRanger01AMO", true),
                new Skin("ImpoRage", true),
                new Skin("ChemSec01SMM", "M"),
                new Skin("ChemWork01GMM", "M"),
                new Skin("Clown01SMY", "M"),
                new Skin("CrisFormage", "M"),new Skin("FilmNoir", "M"),
                new Skin("MimeSMY", "M"),
                new Skin("Stripper01Cutscene", "F"),
                new Skin("Stripper01SFY", "F"),
                new Skin("Stripper02SFY", "F"),
                new Skin("Abigail", "F"),
                new Skin("Acult01AMM", "M"),
                new Skin("Acult01AMO", "M"),
                new Skin("Acult01AMY", "M"),
                new Skin("Acult02AMO", "M"),
                new Skin("Acult02AMY", "M"),
                new Skin("AfriAmer01AMM", "M"),
                new Skin("Agent14", "M"),
                new Skin("AgentCutscene", "M"),
                new Skin("AirHostess01SFY", "F"),
                new Skin("AirWorkerSMY", "M"),
                new Skin("AlDiNapoli", "M"),
                new Skin("AmandaTownley", "F"),
                new Skin("AmmuCity01SMY", "M"),
                new Skin("Andreas", "M"),
                new Skin("AnitaCutscene", "F"),
                new Skin("AntonB", "M"),
                new Skin("ArmBoss01GMM", "M"),
                new Skin("ArmGoon01GMM", "M"),
                new Skin("ArmGoon02GMY", "M"),
                new Skin("ArmLiuet01GMM", "M"),
                new Skin("Ashley", "F"),
                new Skin("AutoShop01SMM", "M"),
                new Skin("Autoshop02SMM", "M"),
                new Skin("AviSchwartzman", "M"),
                new Skin("Azteca01GMY", "M"),
                new Skin("BabyD", "M"),
                new Skin("BallaEast01GMY", "M"),
                new Skin("BallaOrig01GMY", "M"),
                new Skin("Ballas01GFY", "F"),
                new Skin("BallasOG", "M"),
                new Skin("BallaSout01GMY", "M"),
                new Skin("Bankman", "M"),
                new Skin("Bankman01", "M"),
                new Skin("Barman01SMY", "M"),
                new Skin("Barry", "M"),
                new Skin("Bartender01SFY", "F"),
                new Skin("Baygor", "M"),
                new Skin("BayWatch01SFY", "F"),
                new Skin("Beach01AFM", "F"),
                new Skin("Beach01AFY", "F"),
                new Skin("Beach01AMM", "M"),
                new Skin("Beach01AMO", "M"),
                new Skin("Praia01AMY", "M"),
                new Skin("Beach02AMM", "M"),
                new Skin("Beach02AMY", "M"),
                new Skin("Beach03AMY", "M"),
                new Skin("BeachVesp01AMY", "M"),
                new Skin("BeachVesp02AMY", "M"),
                new Skin("Benny", "M"),
                new Skin("BestMen", "M"),
                new Skin("Beverly", "M"),
                new Skin("BevHills01AFM", "F"),
                new Skin("Bevhills01AFY", "F"),
                new Skin("BevHills01AMM", "M"),
                new Skin("BevHills01AMY", "M"),
                new Skin("BevHills02AFM", "F"),
                new Skin("BevHills02AFY", "F"),
                new Skin("BevHills02AMM", "M"),
                new Skin("BevHills02AMY", "M"),
                new Skin("Bevhills03AFY", "F"),
                new Skin("BevHills04AFY", "F"),
                new Skin("BikeHire01", "M"),
                new Skin("BikerChic", "F"),
                new Skin("BoatStaff01F", "F"),
                new Skin("BoatStaff01M", "M"),
                new Skin("BodyBuild01AFM", "F"),
                new Skin("Bouncer01SMM", "M"),
                new Skin("Brad", "M"),
                new Skin("BreakDance01AMY", "M"),
                new Skin("Noiva", "F"),
                new Skin("BurgerDrug", "M"),
                new Skin("BusBoy01SMY", "M"),
                new Skin("Busicas01AMY", "M"),
                new Skin("Business01AFY", "F"),
                new Skin("Business01AMM", "M"),
                new Skin("Business01AMY", "M"),
                new Skin("Business02AFM", "F"),
                new Skin("Business02AFY", "F"),
                new Skin("Business02AMY", "M"),
                new Skin("Business03AFY", "F"),
                new Skin("Business03AMY", "M"),
                new Skin("Business04AFY", "F"),
                new Skin("Busker01SMO", "M"),
                new Skin("Car3Guy1", "M"),
                new Skin("Car3Guy2", "M"),
                new Skin("CarBuyerCutscene", "M"),
                new Skin("CCrew01SMM", "M"),
                new Skin("Chefe de cozinha", "M"),
                new Skin("Chef01SMY", "M"),
                new Skin("Chef2", "M"),
                new Skin("ChiBoss01GMM", "M"),
                new Skin("ChiCold01GMM", "M"),
                new Skin("ChiGoon01GMM", "M"),
                new Skin("ChiGoon02GMM", "M"),
                new Skin("Lasca", "M"),
                new Skin("Argila", "M"),
                new Skin("Cletus", "M"),
                new Skin("CntryBar01SMM", "M"),
                new Skin("ComJane", "F"),
                new Skin("Cyclist01", "M"),
                new Skin("Cyclist01amy", "M"),
                new Skin("Dale", "M"),
                new Skin("DaveNorton", "M"),
                new Skin("Dealer01SMY", "M"),
                new Skin("DebraCS", "F"),
                new Skin("Denise", "M"),
                new Skin("DeniseFriendCutscene", "F"),
                new Skin("Devin", "M"),
                new Skin("DevinSec01SMY", "M"),
                new Skin("DHill01AMY", "M"),
                new Skin("DoaMan", "M"),
                new Skin("Dom", "M"),
                new Skin("Doorman01SMY", "M"),
                new Skin("DownTown01AFM", "F"),
                new Skin("DownTown01AMY", "M"),
                new Skin("Dreyfuss", "M"),
                new Skin("DrFriedlander", "M"),
                new Skin("EastSA01AFM", "F"),
                new Skin("EastSA01AFY", "F"),
                new Skin("EastSA01AMM", "M"),
                new Skin("EastSA02AFM", "F"),
                new Skin("EastSA02AFY", "F"),
                new Skin("EastSa02AMM", "M"),
                new Skin("EastSA02AMY", "M"),
                new Skin("EastSA03AFY", "F"),
                new Skin("EdToh", "M"),
                new Skin("Epsilon01AFY", "F"),
                new Skin("Epsilon01AMY", "M"),
                new Skin("Epsilon02AMY", "M"),
                new Skin("ExArmy01", "M"),
                new Skin("Fabien", "M"),
                new Skin("FamCA01GMY", "M"),
                new Skin("FamDD01", "M"),
                new Skin("FamDNF01GMY", "M"),
                new Skin("FamFor01GMY", "M"),
                new Skin("Families01GFY", "F"),
                new Skin("Farmer01AMM", "M"),
                new Skin("FatBla01AFM", "F"),
                new Skin("FatCult01AFM", "F"),
                new Skin("FatLatin01AMM", "M"),
                new Skin("FatWhite01AFM", "F"),
                new Skin("FemBarberSFM", "F"),
                new Skin("FilmDirector", "M"),
                new Skin("FinGuru01", "M"),
                new Skin("Fitness01AFY", "F"),
                new Skin("Fitness02AFY", "F"),
                new Skin("FosRepCutscene", "M"),
                new Skin("Franklin", "M"),
                new Skin("G", "M"),
                new Skin("Gaffer01SMM", "M"),
                new Skin("Gay01AMY", "M"),
                new Skin("Gay02AMY", "M"),
                new Skin("GenFat01AMM", "M"),
                new Skin("GenFat02AMM", "M"),
                new Skin("GenHot01AFY", "F"),
                new Skin("GenStreet01AFO", "F"),
                new Skin("GenStreet01AMO", "M"),
                new Skin("GenStreet01AMY", "M"),
                new Skin("GenStreet02AMY", "M"),
                new Skin("GenTransportSMM", "M"),
                new Skin("GlenStank01", "M"),
                new Skin("Golfer01AFY", "F"),
                new Skin("Golfer01AMM", "M"),
                new Skin("Golfer01AMY", "M"),
                new Skin("Griff01", "M"),
                new Skin("Grip01SMY", "M"),
                new Skin("Groom", "M"),
                new Skin("GroveStrDlrCutscene", "M"),
                new Skin("GuadalopeCutscene", "F"),
                new Skin("Guido01", "M"),
                new Skin("GunVend01", "M"),
                new Skin("GurkCutscene", "F"),
                new Skin("Hacker", "M"),
                new Skin("HairDress01SMM", "M"),
                new Skin("Hao", "M"),
                new Skin("HasJew01AMM", "M"),
                new Skin("HasJew01AMY", "M"),
                new Skin("HighSec01SMM", "M"),
                new Skin("HighSec02SMM", "M"),
                new Skin("Hiker01AFY", "F"),
                new Skin("Hiker01AMY", "M"),
                new Skin("HillBilly01AMM", "M"),
                new Skin("HillBilly02AMM", "M"),
                new Skin("Hippie01", "M"),
                new Skin("Hippie01AFY", "F"),
                new Skin("Hippy01AMY", "M"),
                new Skin("Hipster01AFY", "F"),
                new Skin("Hipster01AMY", "M"),
                new Skin("Hipster02AFY", "F"),
                new Skin("Hipster02AMY", "M"),
                new Skin("Hipster03AFY", "F"),
                new Skin("Hipster03AMY", "M"),
                new Skin("Hipster04AFY", "F"),
                new Skin("Hooker01SFY", "F"),
                new Skin("Hooker02SFY", "F"),
                new Skin("Hooker03SFY", "F"),
                new Skin("HotPosh01", "F"),
                new Skin("HughCutscene", "M"),
                new Skin("Hunter", "M"),
                new Skin("ImranCutscene", "M"),
                new Skin("Indian01AFO", "F"),
                new Skin("Indian01AFY", "F"),
                new Skin("Indian01AMM", "M"),
                new Skin("Indian01AMY", "M"),
                new Skin("JackHowitzerCutscene", "M"),
                new Skin("Janet", "F"),
                new Skin("JanitorCS", "M"),
                new Skin("JanitorSMM", "M"),
                new Skin("JayNorris", "M"),
                new Skin("Jesus01", "M"),
                new Skin("JetSki01AMY", "M"),
                new Skin("JewelAss", "F"),
                new Skin("JewelAss01", "F"),
                new Skin("JewelSec01", "M"),
                new Skin("JewelThief", "M"),
                new Skin("JimmyBoston", "M"),
                new Skin("JimmyDisanto", "M"),
                new Skin("JoeMinuteman", "M"),
                new Skin("JoeMinutemanCutscene", "M"),
                new Skin("Josef", "M"),
                new Skin("Josh", "M"),
                new Skin("Juggalo01AFY", "F"),
                new Skin("Juggalo01AMY", "M"),
                new Skin("KerryMcintosh", "F"),
                new Skin("KorBoss01GMM", "M"),
                new Skin("Korean01GMY", "M"),
                new Skin("Korean02GMY", "M"),
                new Skin("KorLieut01GMY", "M"),
                new Skin("KTown01AFM", "F"),
                new Skin("KTown01AFO", "F"),
                new Skin("KTown01AMM", "M"),
                new Skin("KTown01AMO", "M"),
                new Skin("KTown01AMY", "M"),
                new Skin("KTown02AFM", "F"),
                new Skin("KTown02AMY", "M"),
                new Skin("LamarDavis", "M"),
                new Skin("LatHandy01SMM", "M"),
                new Skin("Latino01AMY", "M"),
                new Skin("Lazlow", "M"),
                new Skin("LesterCrest", "M"),
                new Skin("LifeInvad01", "M"),
                new Skin("LifeInvad01SMM", "M"),
                new Skin("LifeInvad02", "M"),
                new Skin("LineCookSMM", "M"),
                new Skin("Lost01GFY", "F"),
                new Skin("Lost01GMY", "M"),
                new Skin("Lost02GMY", "M"),
                new Skin("Lost03GMY", "M"),
                new Skin("LSMetro01SMM", "M"),
                new Skin("Magenta", "F"),
                new Skin("Maid01SFM", "F"),
                new Skin("Malibu01AMM", "M"),
                new Skin("Mani", "M"),
                new Skin("Manuel", "M"),
                new Skin("Mariachi01SMM", "M"),
                new Skin("MarkFost", "M"),
                new Skin("Marnie", "F"),
                new Skin("MartinMadrazo", "M"),
                new Skin("Maryann", "F"),
                new Skin("Maude", "F"),
                new Skin("MethHead01AMY", "M"),
                new Skin("MexBoss01GMM", "M"),
                new Skin("MexBoss02GMM", "M"),
                new Skin("MexCntry01AMM", "M"),
                new Skin("MexGang01GMY", "M"),
                new Skin("MexGoon01GMY", "M"),
                new Skin("MexGoon02GMY", "M"),
                new Skin("MexGoon03GMY", "M"),
                new Skin("MexLabor01AMM", "M"),
                new Skin("Mexthug01AMY", "M"),
                new Skin("Michael", "M"),
                new Skin("Migrant01SFY", "F"),
                new Skin("Migrant01SMM", "M"),
                new Skin("MilitaryBum", "M"),
                new Skin("Milton", "M"),
                new Skin("Miranda", "F"),
                new Skin("Mistress", "F"),
                new Skin("Misty01", "F"),
                new Skin("Molly", "F"),
                new Skin("Motox01AMY", "M"),
                new Skin("Motox02AMY", "M"),
                new Skin("MoviePremFemaleCS", "F"),
                new Skin("MoviePremMaleCutscene", "M"),
                new Skin("MovieStar", "F"),
                new Skin("MovPrem01SFY", "F"),
                new Skin("MovPrem01SMM", "M"),
                new Skin("MPros01", "M"),
                new Skin("MrK", "M"),
                new Skin("MrsPhillips", "F"),
                new Skin("MrsThornhill", "F"),
                new Skin("MuscBeac01AMY", "M"),
                new Skin("MusclBeac02AMY", "M"),
                new Skin("Natalia", "F"),
                new Skin("NervousRon", "M"),
                new Skin("Nigel", "M"),
                new Skin("OGBoss01AMM", "M"),
                new Skin("OldMan1A", "M"),
                new Skin("OldMan2", "M"),
                new Skin("Omega", "M"),
                new Skin("Oneil", "M"),
                new Skin("Ortega", "M"),
                new Skin("OscarCutscene", "M"),
                new Skin("Paige", "F"),
                new Skin("Paparazzi", "M"),
                new Skin("Paparazzi01AMM", "M"),
                new Skin("Paper", "M"),
                new Skin("Party01", "M"),
                new Skin("PartyTarget", "M"),
                new Skin("Patricia", "F"),
                new Skin("PoloGoon01GMY", "M"),
                new Skin("PoloGoon02GMY", "M"),
                new Skin("Polynesian01AMM", "M"),
                new Skin("Polynesian01AMY", "M"),
                new Skin("Popov", "M"),
                new Skin("PoppyMich", "F"),
                new Skin("PornDudesCutscene", "M"),
                new Skin("Princess", "F"),
                new Skin("PrologueDriver", "M"),
                new Skin("PrologueHostage01", "F"),
                new Skin("PrologueHostage01AFM", "F"),
                new Skin("PrologueHostage01AMM", "M"),
                new Skin("PrologueMournFemale01", "F"),
                new Skin("PrologueMournMale01", "M"),
                new Skin("RampGang", "M"),
                new Skin("RampHic", "M"),
                new Skin("RampHipster", "M"),
                new Skin("RampMex", "M"),
                new Skin("ReporterCutscene", "M"),
                new Skin("RivalPaparazzi", "M"),
                new Skin("RoadCyc01AMY", "M"),
                new Skin("Robber01SMY", "M"),
                new Skin("RoccoPelosi", "M"),
                new Skin("Runner01AFY", "F"),
                new Skin("Runner01AMY", "M"),
                new Skin("Runner02AMY", "M"),
                new Skin("RurMeth01AFY", "F"),
                new Skin("RurMeth01AMM", "M"),
                new Skin("RussianDrunk", "M"),
                new Skin("Salton01AFM", "F"),
                new Skin("Salton01AFO", "F"),
                new Skin("Salton01AMM", "M"),
                new Skin("Salton01AMO", "M"),
                new Skin("Salton01AMY", "M"),
                new Skin("Salton02AMM", "M"),
                new Skin("Salton03AMM", "M"),
                new Skin("Salton04AMM", "M"),
                new Skin("SalvaBoss01GMY", "M"),
                new Skin("SalvaGoon01GMY", "M"),
                new Skin("SalvaGoon02GMY", "M"),
                new Skin("SalvaGoon03GMY", "M"),
                new Skin("SBikeAMO", "M"),
                new Skin("SCDressy01AFY", "F"),
                new Skin("ScreenWriter", "F"),
                new Skin("ShopHighSFM", "F"),
                new Skin("ShopKeep01", "M"),
                new Skin("ShopLowSFY", "F"),
                new Skin("ShopMaskSMY", "M"),
                new Skin("ShopMidSFY", "F"),
                new Skin("SiemonYetarian", "M"),
                new Skin("Skater01AFY", "F"),
                new Skin("Skater01AMM", "M"),
                new Skin("Skater01AMY", "M"),
                new Skin("Skater02AMY", "M"),
                new Skin("Skidrow01AFM", "M"),
                new Skin("SkidRow01AMM", "M"),
                new Skin("SoCenLat01AMM", "M"),
                new Skin("Solomon", "M"),
                new Skin("SouCent01AFM", "F"),
                new Skin("SouCent01AFO", "F"),
                new Skin("SouCent01AFY", "F"),
                new Skin("SouCent01AMM", "M"),
                new Skin("SouCent01AMO", "M"),
                new Skin("SouCent01AMY", "M"),
                new Skin("SouCent02AFM", "F"),
                new Skin("SouCent02AFO", "F"),
                new Skin("SouCent02AFY", "F"),
                new Skin("SouCent02AMM", "M"),
                new Skin("SouCent02AMO", "M"),
                new Skin("SouCent02AMY", "M"),
                new Skin("SouCent03AFY", "F"),
                new Skin("SouCent03AMM", "M"),
                new Skin("SouCent03AMO", "M"),
                new Skin("SouCent03AMY", "M"),
                new Skin("SouCent04AMM", "M"),
                new Skin("SouCent04AMY", "M"),
                new Skin("SouCentMC01AFM", "F"),
                new Skin("SpyActor", "M"),
                new Skin("SpyActress", "F"),
                new Skin("StagGrm01AMO", "M"),
                new Skin("StBla01AMY", "M"),
                new Skin("StBla02AMY", "M"),
                new Skin("StLat01AMY", "M"),
                new Skin("StLat02AMM", "M"),
                new Skin("Stretch", "M"),
                new Skin("StrPreach01SMM", "M"),
                new Skin("StrPunk01GMY", "M"),
                new Skin("StrPunk02GMY", "M"),
                new Skin("StrVend01SMM", "M"),
                new Skin("StrVend01SMY", "M"),
                new Skin("StWhi01AMY", "M"),
                new Skin("StWhi02AMY", "M"),
                new Skin("SunBathe01AMY", "M"),
                new Skin("Surfer01AMY", "M"),
                new Skin("SweatShop01SFM", "F"),
                new Skin("SweatShop01SFY", "F"),
                new Skin("Talina", "F"),
                new Skin("Tanisha", "F"),
                new Skin("TaoCheng", "M"),
                new Skin("TaosTranslator", "M"),
                new Skin("TapHillBilly", "M"),
                new Skin("Tattoo01AMO", "M"),
                new Skin("Tennis01AMM", "M"),
                new Skin("TennisCoach", "M"),
                new Skin("Terry", "M"),
                new Skin("TomCutscene", "M"),
                new Skin("TomEpsilon", "M"),
                new Skin("Tonya", "F"),
                new Skin("Topless01AFY", "F"),
                new Skin("Tourist01AFM", "F"),
                new Skin("Tourist01AFY", "F"),
                new Skin("Tourist01AMM", "M"),
                new Skin("Tourist02AFY", "F"),
                new Skin("TracyDisanto", "F"),
                new Skin("Tramp01", "M"),
                new Skin("Tramp01AFM", "F"),
                new Skin("Tramp01AMM", "M"),
                new Skin("Tramp01AMO", "M"),
                new Skin("TrampBeac01AFM", "F"),
                new Skin("TrampBeac01AMM", "M"),
                new Skin("TranVest01AMM", "M"),
                new Skin("TranVest02AMM", "M"),
                new Skin("Trevor", "M"),
                new Skin("Trucker01SMM", "M"),
                new Skin("TylerDixon", "M"),
                new Skin("Vagos01GFY", "F"),
                new Skin("VagosFun01", "M"),
                new Skin("VagosSpeak", "M"),
                new Skin("Valet01SMY", "M"),
                new Skin("VinDouche01AMY", "M"),
                new Skin("Vinewood01AFY", "F"),
                new Skin("VineWood01AMY", "M"),
                new Skin("Vinewood02AFY", "F"),
                new Skin("Vinewood02AMY", "M"),
                new Skin("Vinewood03AFY", "F"),
                new Skin("Vinewood03AMY", "M"),
                new Skin("Vinewood04AFY", "F"),
                new Skin("Vinewood04AMY", "M"),
                new Skin("Wade", "M"),
                new Skin("Waiter01SMY", "M"),
                new Skin("WeiCheng", "M"),
                new Skin("WillyFist", "M"),
                new Skin("WinClean01SMY", "M"),
                new Skin("XMech01SMY", "M"),
                new Skin("XMech02SMY", "M"),
                new Skin("Yoga01AFY", "F"),
                new Skin("Yoga01AMY", "M"),
                new Skin("Zimbor", "M"),
                new Skin("Construct01SMY", "M"),
                new Skin("Construct02SMY", "M"),
                new Skin("DockWork01SMM", "M"),
                new Skin("DockWork01SMY", "M"),
                new Skin("DWService01SMY", "M"),
                new Skin("DWService02SMY", "M"),
                new Skin("Factory01SFY", "F"),
                new Skin("Factory01SMY", "M"),
                new Skin("Floyd", "M"),
                new Skin("GarbageSMY", "M"),
                new Skin("Gardener01SMM", "M"),
                new Skin("PestCont01SMY", "M"),
                new Skin("PestContDriver", "M"),
                new Skin("Postal01SMM", "M"),
                new Skin("Postal02SMM", "M"),
                new Skin("Priest", "M"),
                new Skin("BayWatch01SMY", "M"),
                new Skin("Fireman01SMY", "M", TipoFaccao.Medica),
                new Skin("Doctor01SMM", "M", TipoFaccao.Medica),
                new Skin("Autopsy01SMY", "M", TipoFaccao.Medica),
                new Skin("Paramedic01SMM", "M", TipoFaccao.Medica),
                new Skin("Scientist01SMM", "M", TipoFaccao.Medica),
                new Skin("Scrubs01SFY", "F", TipoFaccao.Medica),
                new Skin("Armoured01", "M", TipoFaccao.Policial),
                new Skin("Armoured01SMM", "M", TipoFaccao.Policial),
                new Skin("Armoured02SMM", "M", TipoFaccao.Policial),
                new Skin("ArmyMech01SMY", "M", TipoFaccao.Policial),
                new Skin("BlackOps01SMY", "M", TipoFaccao.Policial),
                new Skin("BlackOps02SMY", "M", TipoFaccao.Policial),
                new Skin("BlackOps03SMY", "M", TipoFaccao.Policial),
                new Skin("Casey", "M", TipoFaccao.Policial),
                new Skin("CIASec01SMM", "M", TipoFaccao.Policial),
                new Skin("Cop01SFY ", "F", TipoFaccao.Policial),
                new Skin("Cop01SMY", "M", TipoFaccao.Policial),
                new Skin("CopCutscene", "M", TipoFaccao.Policial),
                new Skin("FBISuit01", "M", TipoFaccao.Policial),
                new Skin("FIBArchitect", "M", TipoFaccao.Policial),
                new Skin("FIBMugger01", "M", TipoFaccao.Policial),
                new Skin("FIBOffice01SMM", "M", TipoFaccao.Policial),
                new Skin("FIBOffice02SMM", "M", TipoFaccao.Policial),
                new Skin("FIBSec01", "M", TipoFaccao.Policial),
                new Skin("FIBSec01SMM", "M", TipoFaccao.Policial),
                new Skin("HWayCop01SMY", "M", TipoFaccao.Policial),
                new Skin("KarenDaniels", "F", TipoFaccao.Policial),
                new Skin("Marine01SMM", "M", TipoFaccao.Policial),
                new Skin("Marine01SMY", "M", TipoFaccao.Policial),
                new Skin("Marine02SMM", "M", TipoFaccao.Policial),
                new Skin("Marine02SMY", "M", TipoFaccao.Policial),
                new Skin("Marine03SMY", "M", TipoFaccao.Policial),
                new Skin("MerryWeatherCutscene", "M", TipoFaccao.Policial),
                new Skin("Michelle", "F", TipoFaccao.Policial),
                new Skin("Pilot01SMM", "M", TipoFaccao.Policial),
                new Skin("Pilot01SMY", "M", TipoFaccao.Policial),
                new Skin("Pilot02SMM", "M", TipoFaccao.Policial),
                new Skin("PrisGuard01SMM", "M", TipoFaccao.Policial),
                new Skin("PrologueSec01CS", "M", TipoFaccao.Policial),
                new Skin("PrologueSec02", "M", TipoFaccao.Policial),
                new Skin("RampMarineCutscene", "M", TipoFaccao.Policial),
                new Skin("Ranger01SFY", "F", TipoFaccao.Policial),
                new Skin("Ranger01SMY", "M", TipoFaccao.Policial),
                new Skin("Security01SMM", "M", TipoFaccao.Policial),
                new Skin("Sheriff01SFY", "F", TipoFaccao.Policial),
                new Skin("Sheriff01SMY", "M", TipoFaccao.Policial),
                new Skin("SnowCop01SMM", "M", TipoFaccao.Policial),
                new Skin("SteveHains", "M", TipoFaccao.Policial),
                new Skin("SWAT01SMY", "M", TipoFaccao.Policial),
                new Skin("TrafficWarden", "M", TipoFaccao.Policial),
                new Skin("UndercoverCopCutscene", "M", TipoFaccao.Policial),
                new Skin("UPS01SMM", "M", TipoFaccao.Policial),
                new Skin("UPS02SMM", "M", TipoFaccao.Policial),
                new Skin("USCG01SMY", "M", TipoFaccao.Policial),
            };
        }

        public static void CarregarConcessionarias()
        {
            Global.Concessionarias = new List<Concessionaria>()
            {
                new Concessionaria()
                {
                    Nome = "Concessionária de Carros e Motos",
                    Tipo = TipoPreco.CarrosMotos,
                    PosicaoCompra = new Vector3(-38.63479, -1109.706, 26.43781),
                    PosicaoSpawn = new Vector3(-59.85905, -1106.017, 26.01114),
                    RotacaoSpawn = new Vector3(0.6498904, 0.5028602, 73.53305),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Barcos",
                    Tipo = TipoPreco.Barcos,
                    PosicaoCompra = new Vector3(-787.1262, -1354.725, 5.150271),
                    PosicaoSpawn = new Vector3(-805.9193, -1418.638, 0.3696117),
                    RotacaoSpawn = new Vector3(0, 0, 209.8481),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Helicópteros",
                    Tipo = TipoPreco.Helicopteros,
                    PosicaoCompra = new Vector3(-753.5287, -1512.43, 5.020952),
                    PosicaoSpawn = new Vector3(- 745.4902, -1468.695, 5.099712),
                    RotacaoSpawn = new Vector3(0, 0, 328.6675),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária Industrial",
                    Tipo = TipoPreco.Industrial,
                    PosicaoCompra = new Vector3(473.9496, -1951.891, 24.6132),
                    PosicaoSpawn = new Vector3(468.1417, -1957.425, 24.72257),
                    RotacaoSpawn = new Vector3(0, 0, 208.0628),
                },
                new Concessionaria()
                {
                    Nome = "Concessionária de Aviões",
                    Tipo = TipoPreco.Avioes,
                    PosicaoCompra = new Vector3(1725.616, 3291.571, 41.19078),
                    PosicaoSpawn = new Vector3(1712.708, 3252.634, 41.67871),
                    RotacaoSpawn = new Vector3(0, 0, 122.1655),
                },
            };

            foreach (var c in Global.Concessionarias)
            {
                NAPI.TextLabel.CreateTextLabel(c.Nome, c.PosicaoCompra, 5, 2, 0, new Color(254, 189, 12));
                NAPI.TextLabel.CreateTextLabel("Use /vcomprar", new Vector3(c.PosicaoCompra.X, c.PosicaoCompra.Y, c.PosicaoCompra.Z - 0.1), 5, 1, 0, new Color(255, 255, 255));
            }
        }

        public static string ObterDisplayEnum(Enum valor)
        {
            var fi = valor.GetType().GetField(valor.ToString());
            var attributes = (DisplayAttribute[])fi.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes?.Length > 0)
                return attributes.FirstOrDefault().Name;
            return valor.ToString();
        }

        public static void CarregarEmpregos()
        {
            Global.Empregos = new List<Emprego>()
            {
                new Emprego()
                {
                    Tipo = TipoEmprego.Taxista,
                    Posicao = new Vector3(895.0308, -179.1359, 74.70036),
                },
            };

            foreach (var c in Global.Empregos)
            {
                var nome = ObterDisplayEnum(c.Tipo);
                NAPI.TextLabel.CreateTextLabel($"Emprego de {nome}", c.Posicao, 5, 2, 0, new Color(254, 189, 12));
                NAPI.TextLabel.CreateTextLabel($"Use /emprego para se tornar um {nome.ToLower()}", new Vector3(c.Posicao.X, c.Posicao.Y, c.Posicao.Z - 0.1), 5, 1, 0, new Color(255, 255, 255));
            }
        }

        public static void EnviarMensagemEmprego(TipoEmprego tipo, string mensagem)
        {
            foreach (var pl in Global.PersonagensOnline.Where(x => x.Emprego == (int)tipo && x.IsEmTrabalho))
                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem);
        }
    }
}