using GTANetworkAPI;
using InfiniteRoleplay.Entities;
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
            player.Position = new Vector3(p.PosX, p.PosY, p.PosZ);
            player.Health = p.Vida;
            player.Armor = p.Colete;
            player.SetSkin((uint)p.Skin);
            p.SetDinheiro();

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
            }

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == p.Codigo);
                personagem.Online = isOnline;
                personagem.Skin = player.Model;
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
                context.Personagens.Update(personagem);
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
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{ObterNomeIC(p)} diz: {message}");
                                break;
                            case TipoMensagemJogo.ChatICGrito:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"{ObterNomeIC(p)} grita: {message}");
                                break;
                            case TipoMensagemJogo.Me:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#C2A2DA}" + $"{ObterNomeIC(p)} {message}");
                                break;
                            case TipoMensagemJogo.Do:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#C2A2DA}" + $"{message} (( {ObterNomeIC(p)} ))");
                                break;
                            case TipoMensagemJogo.ChatOOC:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#bababa}" + $"(( {ObterNomeIC(p)} [{p.ID}]: {message} ))");
                                break;
                            case TipoMensagemJogo.ChatICBaixo:
                                EnviarMensagem(target, TipoMensagem.Nenhum, chatMessageColor + $"[BAIXO] {ObterNomeIC(p)} diz: {message}");
                                break;
                            case TipoMensagemJogo.Megafone:
                                EnviarMensagem(target, TipoMensagem.Nenhum, "!{#F2FF43}" + $"[MEGAFONE] {ObterNomeIC(p)} diz: {message}");
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

        public static string ObterNomePadraoBlip(int tipo, string nome)
        {
            switch (tipo)
            {
                case 361:
                    return "Posto de Gasolina";
            }

            return nome;
        }

        public static string ObterNomeIC(Personagem p) => p.Nome;

        public static void MostrarStats(Client player, Personagem p)
        {
            EnviarMensagem(player, TipoMensagem.Titulo, $"Informações de {p.Nome} [{p.Codigo}]");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"OOC: {p.UsuarioBD.Nome} | SocialClub: {p.Player.SocialClubName} | Staff: {p.UsuarioBD.Staff}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Registro: {p.DataRegistro.ToString()} | Tempo Conectado: {p.TempoConectado}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Sexo: {p.Sexo} | Nascimento: {p.DataNascimento.ToShortDateString()} | Dinheiro: ${p.Dinheiro.ToString("N0")}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Skin: {((PedHash)p.Player.Model).ToString()} | Vida: {p.Player.Health} | Colete: {p.Player.Armor}");

            if (p.Faccao > 0)
                EnviarMensagem(player, TipoMensagem.Nenhum, $"Facção: {p.FaccaoBD?.Nome} [{p.Faccao}] | Rank: {p.RankBD?.Nome} [{p.Rank}]");

            if (p.Propriedades.Count > 0)
            {
                EnviarMensagem(player, TipoMensagem.Titulo, $"Propriedades de {p.Nome} [{p.Codigo}]");
                foreach (var prop in p.Propriedades)
                    EnviarMensagem(player, TipoMensagem.Nenhum, $"Código: {prop.Codigo} | Valor: ${prop.Valor.ToString("N0")}");
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
            }
            return new Vector3();
        }
    }
}