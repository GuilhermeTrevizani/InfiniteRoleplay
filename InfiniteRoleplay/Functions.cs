using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace InfiniteRoleplay
{
    public static class Functions
    {
        public static void EnviarMensagem(Client player, TipoMensagem tipoMensagem, string mensagem)
        {
            var strTipo = string.Empty;
            if (tipoMensagem == TipoMensagem.Sucesso)
                strTipo = "!{#6EB469}<!> ~w~";
            else if (tipoMensagem == TipoMensagem.Erro)
                strTipo = "!{#FF6A4D}<!> ~w~";
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

        public static void LogarPersonagem(Client player, Personagem p)
        {
            p.Convites = new List<Models.Convite>();
            player.Name = $"{p.Nome} [{p.ID}]";
            player.Dimension = (uint)p.Dimensao;
            p.IPLs = JsonConvert.DeserializeObject<List<string>>(p.IPL);
            foreach (var ipl in p.IPLs)
                NAPI.ClientEvent.TriggerClientEvent(player, "setIPL", ipl);
            player.Position = new Vector3(p.PosX, p.PosY, p.PosZ);
            player.Health = p.Vida;
            player.Armor = p.Colete;
            player.SetSkin(NAPI.Util.PedNameToModel(p.Skin));
            p.SetDinheiro();

            using (var context = new RoleplayContext())
                p.Contatos = context.PersonagensContatos.Where(x => x.Codigo == p.Codigo).ToList();

            if (Global.PersonagensOnline.Count > Global.Parametros.RecordeOnline)
            {
                Global.Parametros.RecordeOnline = Global.PersonagensOnline.Count;
                GravarParametros();
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "logarPersonagem");
        }

        public static Personagem ObterPersonagem(Client player)
        {
            return Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
        }

        public static Personagem ObterPersonagemPorIdNome(Client player, string idNome, bool isPodeProprioPlayer = true)
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

        public static void SalvarPersonagem(Client player, bool isOnline = true)
        {
            var p = ObterPersonagem(player);
            if ((p?.Codigo ?? 0) == 0)
                return;

            var dif = DateTime.Now - p.DataUltimaVerificacao;
            if (dif.TotalMinutes >= 1)
            {
                p.TempoConectado++;
                p.DataUltimaVerificacao = DateTime.Now;

                if (p.TempoConectado % 60 == 0)
                {
                    var salario = 0;
                    if (p.Faccao > 0)
                    {
                        salario += p.RankBD.Salario;
                    }
                    else
                    {
                        salario += Global.Parametros.ValorSalarioDesemprego;
                    }

                    p.Banco += salario;
                    if (salario > 0)
                        EnviarMensagem(player, TipoMensagem.Sucesso, $"Seu salário de ${salario:N0} foi depositado no banco!");
                }
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

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == p.Codigo);
                personagem.Online = isOnline;
                personagem.Skin = ((PedHash)player.Model).ToString();
                personagem.PosX = player.Position.X;
                personagem.PosY = player.Position.Y;
                personagem.PosZ = player.Position.Z;
                personagem.Vida = player.Health;
                personagem.Colete = player.Armor;
                personagem.Dimensao = player.Dimension;
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
                context.Personagens.Update(personagem);

                context.Database.ExecuteSqlCommand($"DELETE FROM PersonagensContatos WHERE Codigo = {p.Codigo}");
                context.PersonagensContatos.AddRange(p.Contatos);

                context.SaveChanges();
            }
        }

        public static void SendMessageToNearbyPlayers(Client player, string message, TipoMensagemJogo type, float range, bool excludePlayer = false)
        {
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
            float distanceGap = range / 5;

            List<Client> targetList = NAPI.Pools.GetAllPlayers().Where(x => Global.PersonagensOnline.Any(y => y.UsuarioBD.SocialClubRegistro == x.SocialClubName) && x.Dimension == player.Dimension).ToList();

            foreach (Client target in targetList)
            {
                if (player != target || (player == target && !excludePlayer))
                {
                    float distance = player.Position.DistanceTo(target.Position);

                    if (distance <= range)
                    {
                        string chatMessageColor = GetChatMessageColor(distance, distanceGap);

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
            string color = null;
            if (distance < distanceGap)
                color = Constants.COLOR_CHAT_CLOSE;
            else if (distance < distanceGap * 2)
                color = Constants.COLOR_CHAT_NEAR;
            else if (distance < distanceGap * 3)
                color = Constants.COLOR_CHAT_MEDIUM;
            else if (distance < distanceGap * 4)
                color = Constants.COLOR_CHAT_FAR;
            else
                color = Constants.COLOR_CHAT_LIMIT;
            return color;
        }

        public static void MostrarStats(Client player, Personagem p)
        {
            EnviarMensagem(player, TipoMensagem.Titulo, $"Informações de {p.Nome} [{p.Codigo}]");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"OOC: {p.UsuarioBD.Nome} | SocialClub: {p.Player.SocialClubName} | Staff: {p.UsuarioBD.Staff}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Registro: {p.DataRegistro.ToString()} | Tempo Conectado: {p.TempoConectado} | Celular: {p.Celular}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Sexo: {p.Sexo} | Nascimento: {p.DataNascimento.ToShortDateString()} | Dinheiro: ${p.Dinheiro:N0} | Banco: ${p.Banco:N0}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Skin: {((PedHash)p.Player.Model).ToString()} | Vida: {p.Player.Health} | Colete: {p.Player.Armor}");

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

        public static bool VerificarBanimento(Client player, Banimento ban)
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

                NAPI.ClientEvent.TriggerClientEvent(player, "mensagem", $"Você está banido{strBan}<br/>Data: {ban.Data.ToString()} | Motivo: {ban.Motivo} | Staff: {staff}");
            }

            player.Kick();
            return false;
        }

        public static void GravarLog(TipoLog tipo, string descricao, Personagem origem, Personagem destino)
        {
            using (var context = new RoleplayContext())
            {
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
        }

        public static void GravarParametros()
        {
            using (var context = new RoleplayContext())
            {
                context.Parametros.Update(Global.Parametros);
                context.SaveChanges();
            }
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
            switch (tipo)
            {
                case TipoInterior.Motel:
                    return new Vector3(151.2564, -1007.868, -98.99999);
                case TipoInterior.CasaBaixa:
                    return new Vector3(265.9522, -1007.485, -101.0085);
                case TipoInterior.CasaMedia:
                    return new Vector3(346.4499, -1012.996, -99.19622);
                case TipoInterior.IntegrityWay28:
                    return new Vector3(-31.34092, -594.9429, 80.0309);
                case TipoInterior.IntegrityWay30:
                    return new Vector3(-17.61359, -589.3938, 90.11487);
                case TipoInterior.DellPerroHeights4:
                    return new Vector3(-1452.225, -540.4642, 74.04436);
                case TipoInterior.DellPerroHeights7:
                    return new Vector3(-1451.26, -523.9634, 56.92898);
                case TipoInterior.RichardMajestic2:
                    return new Vector3(-912.6351, -364.9724, 114.2748);
                case TipoInterior.TinselTowers42:
                    return new Vector3(-603.1113, 58.93406, 98.20017);
                case TipoInterior.EclipseTowers3:
                    return new Vector3(-785.1537, 323.8156, 211.9973);
                case TipoInterior.WildOatsDrive3655:
                    return new Vector3(-174.3753, 497.3086, 137.6669);
                case TipoInterior.NorthConkerAvenue2044:
                    return new Vector3(341.9306, 437.7751, 149.3901);
                case TipoInterior.NorthConkerAvenue2045:
                    return new Vector3(373.5803, 423.7043, 145.9078);
                case TipoInterior.HillcrestAvenue2862:
                    return new Vector3(-682.3693, 592.2678, 145.393);
                case TipoInterior.HillcrestAvenue2868:
                    return new Vector3(-758.4348, 618.8454, 144.1539);
                case TipoInterior.HillcrestAvenue2874:
                    return new Vector3(-859.7643, 690.8358, 152.8607);
                case TipoInterior.WhispymoundDrive2677:
                    return new Vector3(117.209, 559.8086, 184.3048);
                case TipoInterior.MadWayneThunder2133:
                    return new Vector3(-1289.775, 449.3125, 97.90256);
                case TipoInterior.Modern1Apartment:
                    return new Vector3(-786.8663, 315.7642, 217.6385);
                case TipoInterior.Modern2Apartment:
                    return new Vector3(-786.9563, 315.6229, 187.9136);
                case TipoInterior.Modern3Apartment:
                    return new Vector3(-774.0126, 342.0428, 196.6864);
                case TipoInterior.Mody1Apartment:
                    return new Vector3(-787.0749, 315.8198, 217.6386);
                case TipoInterior.Mody2Apartment:
                    return new Vector3(-786.8195, 315.5634, 187.9137);
                case TipoInterior.Mody3Apartment:
                    return new Vector3(-774.1382, 342.0316, 196.6864);
                case TipoInterior.Vibrant1Apartment:
                    return new Vector3(-786.6245, 315.6175, 217.6385);
                case TipoInterior.Vibrant2Apartment:
                    return new Vector3(-786.9584, 315.7974, 187.9135);
                case TipoInterior.Vibrant3Apartment:
                    return new Vector3(-774.0223, 342.1718, 196.6863);
                case TipoInterior.Sharp1Apartment:
                    return new Vector3(-787.0902, 315.7039, 217.6384);
                case TipoInterior.Sharp2Apartment:
                    return new Vector3(-787.0155, 315.7071, 187.9135);
                case TipoInterior.Sharp3Apartment:
                    return new Vector3(-773.8976, 342.1525, 196.6863);
                case TipoInterior.Monochrome1Apartment:
                    return new Vector3(-786.9887, 315.7393, 217.6386);
                case TipoInterior.Monochrome2Apartment:
                    return new Vector3(-786.8809, 315.6634, 187.9136);
                case TipoInterior.Monochrome3Apartment:
                    return new Vector3(-774.0675, 342.0773, 196.6864);
                case TipoInterior.Seductive1Apartment:
                    return new Vector3(-787.1423, 315.6943, 217.6384);
                case TipoInterior.Seductive2Apartment:
                    return new Vector3(-787.0961, 315.815, 187.9135);
                case TipoInterior.Seductive3Apartment:
                    return new Vector3(-773.9552, 341.9892, 196.6862);
                case TipoInterior.Regal1Apartment:
                    return new Vector3(-787.029, 315.7113, 217.6385);
                case TipoInterior.Regal2Apartment:
                    return new Vector3(-787.0574, 315.6567, 187.9135);
                case TipoInterior.Regal3Apartment:
                    return new Vector3(-774.0109, 342.0965, 196.6863);
                case TipoInterior.Aqua1Apartment:
                    return new Vector3(-786.9469, 315.5655, 217.6383);
                case TipoInterior.Aqua2Apartment:
                    return new Vector3(-786.9756, 315.723, 187.9134);
                case TipoInterior.Aqua3Apartment:
                    return new Vector3(-774.0349, 342.0296, 196.6862);
                case TipoInterior.ArcadiusExecutiveRich:
                    return new Vector3(-141.1987, -620.913, 168.8205);
                case TipoInterior.ArcadiusExecutiveCool:
                    return new Vector3(-141.5429, -620.9524, 168.8204);
                case TipoInterior.ArcadiusExecutiveContrast:
                    return new Vector3(-141.2896, -620.9618, 168.8204);
                case TipoInterior.ArcadiusOldSpiceWarm:
                    return new Vector3(-141.4966, -620.8292, 168.8204);
                case TipoInterior.ArcadiusOldSpiceClassical:
                    return new Vector3(-141.3997, -620.9006, 168.8204);
                case TipoInterior.ArcadiusOldSpiceVintage:
                    return new Vector3(-141.5361, -620.9186, 168.8204);
                case TipoInterior.ArcadiusPowerBrokerIce:
                    return new Vector3(-141.392, -621.0451, 168.8204);
                case TipoInterior.ArcadiusPowerBrokeConservative:
                    return new Vector3(-141.1945, -620.8729, 168.8204);
                case TipoInterior.ArcadiusPowerBrokePolished:
                    return new Vector3(-141.4924, -621.0035, 168.8205);
                case TipoInterior.MazeBankExecutiveRich:
                    return new Vector3(-75.8466, -826.9893, 243.3859);
                case TipoInterior.MazeBankExecutiveCool:
                    return new Vector3(-75.49945, -827.05, 243.386);
                case TipoInterior.MazeBankExecutiveContrast:
                    return new Vector3(-75.49827, -827.1889, 243.386);
                case TipoInterior.MazeBankOldSpiceWarm:
                    return new Vector3(-75.44054, -827.1487, 243.3859);
                case TipoInterior.MazeBankOldSpiceClassical:
                    return new Vector3(-75.63942, -827.1022, 243.3859);
                case TipoInterior.MazeBankOldSpiceVintage:
                    return new Vector3(-75.47446, -827.2621, 243.386);
                case TipoInterior.MazeBankPowerBrokerIce:
                    return new Vector3(-75.56978, -827.1152, 243.3859);
                case TipoInterior.MazeBankPowerBrokeConservative:
                    return new Vector3(-75.51953, -827.0786, 243.3859);
                case TipoInterior.MazeBankPowerBrokePolished:
                    return new Vector3(-75.41915, -827.1118, 243.3858);
                case TipoInterior.LomBankExecutiveRich:
                    return new Vector3(-1579.756, -565.0661, 108.523);
                case TipoInterior.LomBankExecutiveCool:
                    return new Vector3(-1579.678, -565.0034, 108.5229);
                case TipoInterior.LomBankExecutiveContrast:
                    return new Vector3(-1579.583, -565.0399, 108.5229);
                case TipoInterior.LomBankOldSpiceWarm:
                    return new Vector3(-1579.702, -565.0366, 108.5229);
                case TipoInterior.LomBankOldSpiceClassical:
                    return new Vector3(-1579.643, -564.9685, 108.5229);
                case TipoInterior.LomBankOldSpiceVintage:
                    return new Vector3(-1579.681, -565.0003, 108.523);
                case TipoInterior.LomBankPowerBrokerIce:
                    return new Vector3(-1579.677, -565.0689, 108.5229);
                case TipoInterior.LomBankPowerBrokeConservative:
                    return new Vector3(-1579.708, -564.9634, 108.5229);
                case TipoInterior.LomBankPowerBrokePolished:
                    return new Vector3(-1579.693, -564.8981, 108.5229);
                case TipoInterior.MazeBankWestExecutiveRich:
                    return new Vector3(-1392.667, -480.4736, 72.04217);
                case TipoInterior.MazeBankWestExecutiveCool:
                    return new Vector3(-1392.542, -480.4011, 72.04211);
                case TipoInterior.MazeBankWestExecutiveContrast:
                    return new Vector3(-1392.626, -480.4856, 72.04212);
                case TipoInterior.MazeBankWestOldSpiceWarm:
                    return new Vector3(-1392.617, -480.6363, 72.04208);
                case TipoInterior.MazeBankWestOldSpiceClassical:
                    return new Vector3(-1392.532, -480.7649, 72.04207);
                case TipoInterior.MazeBankWestOldSpiceVintage:
                    return new Vector3(-1392.611, -480.5562, 72.04214);
                case TipoInterior.MazeBankWestPowerBrokerIce:
                    return new Vector3(-1392.563, -480.549, 72.0421);
                case TipoInterior.MazeBankWestPowerBrokeConservative:
                    return new Vector3(-1392.528, -480.475, 72.04206);
                case TipoInterior.MazeBankWestPowerBrokePolished:
                    return new Vector3(-1392.416, -480.7485, 72.04207);
            }
            return new Vector3();
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
                players = players.Where(x => x.IsTrabalhoFaccao);

            foreach (var pl in players)
                EnviarMensagem(pl.Player, TipoMensagem.Nenhum, (isCorFaccao ? "!{#" + pl.FaccaoBD.Cor + "}" : string.Empty) + mensagem);
        }

        public static void ComprarVeiculo(Client player, string erro)
        {
            var p = ObterPersonagem(player);
            if (p == null)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == (int)TipoPonto.Concessionaria && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de compra de veículos!");
                return;
            }

            var veiculos = Global.Precos.Where(x => x.Tipo == (int)TipoPreco.Veiculo).OrderBy(x => x.Nome).Select(x => new
            {
                x.Nome,
                Preco = $"${x.Valor:N0}",
            }).ToList();

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoVComprar", veiculos, erro);
        }

        public static string GerarPlacaVeiculo()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            var random = new Random();
            return $"{chars[random.Next(25)]}{chars[random.Next(25)]}{random.Next(0, 99999).ToString().PadLeft(5, '0')}{chars[random.Next(25)]}";
        }

        public static void AbrirCelular(Client player, string msg, int tipoMsg)
        {
            var p = ObterPersonagem(player);
            if ((p?.Celular ?? 0) == 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um celular!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "abrirCelular", p.Contatos.OrderBy(x => x.Nome).ToList(), msg, tipoMsg);
        }

        public static void VisualizarMultas(Client player, string erro)
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

            using (var context = new RoleplayContext())
            {
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
        }

        public static List<string> ObterIPLsPorInterior(TipoInterior tipo)
        {
            switch (tipo)
            {
                case TipoInterior.Modern1Apartment:
                    return new List<string> { "apa_v_mp_h_01_a" };
                case TipoInterior.Modern2Apartment:
                    return new List<string> { "apa_v_mp_h_01_c" };
                case TipoInterior.Modern3Apartment:
                    return new List<string> { "apa_v_mp_h_01_b" };
                case TipoInterior.Mody1Apartment:
                    return new List<string> { "apa_v_mp_h_02_a" };
                case TipoInterior.Mody2Apartment:
                    return new List<string> { "apa_v_mp_h_02_c" };
                case TipoInterior.Mody3Apartment:
                    return new List<string> { "apa_v_mp_h_02_b" };
                case TipoInterior.Vibrant1Apartment:
                    return new List<string> { "apa_v_mp_h_03_a" };
                case TipoInterior.Vibrant2Apartment:
                    return new List<string> { "apa_v_mp_h_03_c" };
                case TipoInterior.Vibrant3Apartment:
                    return new List<string> { "apa_v_mp_h_03_b" };
                case TipoInterior.Sharp1Apartment:
                    return new List<string> { "apa_v_mp_h_04_a" };
                case TipoInterior.Sharp2Apartment:
                    return new List<string> { "apa_v_mp_h_04_c" };
                case TipoInterior.Sharp3Apartment:
                    return new List<string> { "apa_v_mp_h_04_b" };
                case TipoInterior.Monochrome1Apartment:
                    return new List<string> { "apa_v_mp_h_05_a" };
                case TipoInterior.Monochrome2Apartment:
                    return new List<string> { "apa_v_mp_h_05_c" };
                case TipoInterior.Monochrome3Apartment:
                    return new List<string> { "apa_v_mp_h_05_b" };
                case TipoInterior.Seductive1Apartment:
                    return new List<string> { "apa_v_mp_h_06_a" };
                case TipoInterior.Seductive2Apartment:
                    return new List<string> { "apa_v_mp_h_06_c" };
                case TipoInterior.Seductive3Apartment:
                    return new List<string> { "apa_v_mp_h_06_b" };
                case TipoInterior.Regal1Apartment:
                    return new List<string> { "apa_v_mp_h_07_a" };
                case TipoInterior.Regal2Apartment:
                    return new List<string> { "apa_v_mp_h_07_c" };
                case TipoInterior.Regal3Apartment:
                    return new List<string> { "apa_v_mp_h_07_b" };
                case TipoInterior.Aqua1Apartment:
                    return new List<string> { "apa_v_mp_h_08_a" };
                case TipoInterior.Aqua2Apartment:
                    return new List<string> { "apa_v_mp_h_08_c" };
                case TipoInterior.Aqua3Apartment:
                    return new List<string> { "apa_v_mp_h_08_b" };
                case TipoInterior.ArcadiusExecutiveRich:
                    return new List<string> { "ex_dt1_02_office_02b" };
                case TipoInterior.ArcadiusExecutiveCool:
                    return new List<string> { "ex_dt1_02_office_02c" };
                case TipoInterior.ArcadiusExecutiveContrast:
                    return new List<string> { "ex_dt1_02_office_02a" };
                case TipoInterior.ArcadiusOldSpiceWarm:
                    return new List<string> { "ex_dt1_02_office_01a" };
                case TipoInterior.ArcadiusOldSpiceClassical:
                    return new List<string> { "ex_dt1_02_office_01b" };
                case TipoInterior.ArcadiusOldSpiceVintage:
                    return new List<string> { "ex_dt1_02_office_01c" };
                case TipoInterior.ArcadiusPowerBrokerIce:
                    return new List<string> { "ex_dt1_02_office_03a" };
                case TipoInterior.ArcadiusPowerBrokeConservative:
                    return new List<string> { "ex_dt1_02_office_03b" };
                case TipoInterior.ArcadiusPowerBrokePolished:
                    return new List<string> { "ex_dt1_02_office_03c" };
                case TipoInterior.MazeBankExecutiveRich:
                    return new List<string> { "ex_dt1_11_office_02b" };
                case TipoInterior.MazeBankExecutiveCool:
                    return new List<string> { "ex_dt1_11_office_02c" };
                case TipoInterior.MazeBankExecutiveContrast:
                    return new List<string> { "ex_dt1_11_office_02a" };
                case TipoInterior.MazeBankOldSpiceWarm:
                    return new List<string> { "ex_dt1_11_office_01a" };
                case TipoInterior.MazeBankOldSpiceClassical:
                    return new List<string> { "ex_dt1_11_office_01b" };
                case TipoInterior.MazeBankOldSpiceVintage:
                    return new List<string> { "ex_dt1_11_office_01c" };
                case TipoInterior.MazeBankPowerBrokerIce:
                    return new List<string> { "ex_dt1_11_office_03a" };
                case TipoInterior.MazeBankPowerBrokeConservative:
                    return new List<string> { "ex_dt1_11_office_03b" };
                case TipoInterior.MazeBankPowerBrokePolished:
                    return new List<string> { "ex_dt1_11_office_03c" };
                case TipoInterior.LomBankExecutiveRich:
                    return new List<string> { "ex_sm_13_office_02b" };
                case TipoInterior.LomBankExecutiveCool:
                    return new List<string> { "ex_sm_13_office_02c" };
                case TipoInterior.LomBankExecutiveContrast:
                    return new List<string> { "ex_sm_13_office_02a" };
                case TipoInterior.LomBankOldSpiceWarm:
                    return new List<string> { "ex_sm_13_office_01a" };
                case TipoInterior.LomBankOldSpiceClassical:
                    return new List<string> { "ex_sm_13_office_01b" };
                case TipoInterior.LomBankOldSpiceVintage:
                    return new List<string> { "ex_sm_13_office_01c" };
                case TipoInterior.LomBankPowerBrokerIce:
                    return new List<string> { "ex_sm_13_office_03a" };
                case TipoInterior.LomBankPowerBrokeConservative:
                    return new List<string> { "ex_sm_13_office_03b" };
                case TipoInterior.LomBankPowerBrokePolished:
                    return new List<string> { "ex_sm_13_office_03c" };
                case TipoInterior.MazeBankWestExecutiveRich:
                    return new List<string> { "ex_sm_15_office_02b" };
                case TipoInterior.MazeBankWestExecutiveCool:
                    return new List<string> { "ex_sm_15_office_02c" };
                case TipoInterior.MazeBankWestExecutiveContrast:
                    return new List<string> { "ex_sm_15_office_02a" };
                case TipoInterior.MazeBankWestOldSpiceWarm:
                    return new List<string> { "ex_sm_15_office_01a" };
                case TipoInterior.MazeBankWestOldSpiceClassical:
                    return new List<string> { "ex_sm_15_office_01b" };
                case TipoInterior.MazeBankWestOldSpiceVintage:
                    return new List<string> { "ex_sm_15_office_01c" };
                case TipoInterior.MazeBankWestPowerBrokerIce:
                    return new List<string> { "ex_sm_15_office_03a" };
                case TipoInterior.MazeBankWestPowerBrokeConservative:
                    return new List<string> { "ex_sm_15_office_03b" };
                case TipoInterior.MazeBankWestPowerBrokePolished:
                    return new List<string> { "ex_sm_15_office_03c" };
            }

            return new List<string>();
        }

        public static bool ChecarAnimacoes(Client player)
        {
            if (player.IsInVehicle)
            {
                EnviarMensagem(player, TipoMensagem.Erro, "Você não pode utilizar comandos de animação em um veículo!");
                return false;
            }

            // Verificar aqui status de ferido, impossibilitado de fazer alguma animação

            player.StopAnimation();
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
                        EnviarMensagemTipoFaccao(tipoFaccao, "Ligação 911", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"De: ~w~{p.Celular}", true, true);
                        EnviarMensagemTipoFaccao(tipoFaccao, $"Mensagem: ~w~{message}", true, true);

                        p.LimparLigacao();
                    }
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

            if ((canal == 911 || canal == 912) && !p.IsTrabalhoFaccao)
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
    }
}