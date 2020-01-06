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
            float distanceGap = range / 5;

            List<Client> targetList = NAPI.Pools.GetAllPlayers().Where(p => Global.PersonagensOnline.Any(x => x.UsuarioBD.SocialClubRegistro == p.SocialClubName) && p.Dimension == player.Dimension).ToList();

            foreach (Client target in targetList)
            {
                if (player != target || (player == target && !excludePlayer))
                {
                    float distance = player.Position.DistanceTo(target.Position);

                    if (distance <= range)
                    {
                        string chatMessageColor = GetChatMessageColor(distance, distanceGap);

                        var p = Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClubRegistro == player.SocialClubName);
                        switch (type)
                        {
                            case TipoMensagemJogo.ChatICNormal:
                                target.SendChatMessage(chatMessageColor + $"{ObterNomeIC(p)} diz: {message}");
                                break;
                            case TipoMensagemJogo.ChatICGrito:
                                target.SendChatMessage(chatMessageColor + $"{ObterNomeIC(p)} grita: {message}");
                                break;
                            case TipoMensagemJogo.Me:
                                target.SendChatMessage("!{#C2A2DA}" + $"{ObterNomeIC(p)} {message}");
                                break;
                            case TipoMensagemJogo.Do:
                                target.SendChatMessage("!{#C2A2DA}" + $"{message} (( {ObterNomeIC(p)} ))");
                                break;
                            case TipoMensagemJogo.ChatOOC:
                                target.SendChatMessage("!{#bababa}" + $"(( {ObterNomeIC(p)} [{p.ID}]: {message} ))");
                                break;
                            case TipoMensagemJogo.ChatICBaixo:
                                target.SendChatMessage(chatMessageColor + $"{ObterNomeIC(p)} diz [baixo]: {message}");
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

        public static string ObterNomeIC(Personagem p)
        {
            return p.Nome;
        }

        public static void MostrarStats(Client player, Personagem p)
        {
            EnviarMensagem(player, TipoMensagem.Titulo, $"Informações de {p.Nome} [{p.Codigo}]");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"OOC: {p.UsuarioBD.Nome} | SocialClub: {p.Player.SocialClubName} | Staff: {p.UsuarioBD.Staff}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Registro: {p.DataRegistro.ToString()} | Tempo Conectado: {p.TempoConectado}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Sexo: {p.Sexo} | Nascimento: {p.DataNascimento.ToShortDateString()} | Dinheiro: {p.Dinheiro.ToString("N0")}");
            EnviarMensagem(player, TipoMensagem.Nenhum, $"Skin: {((PedHash)p.Player.Model).ToString()} | Vida: {p.Player.Health} | Colete: {p.Player.Armor}");

            if (p.Faccao > 0)
                EnviarMensagem(player, TipoMensagem.Nenhum, $"Facção: {p.FaccaoBD?.Nome} [{p.Faccao}] | Rank: {p.RankBD?.Nome} [{p.Rank}]");
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
            for(var i=1; i <= NAPI.Server.GetMaxPlayers(); i++)
            {
                if (Global.PersonagensOnline.Any(x => x.ID == i))
                    continue;

                return i;
            }

            return 1;
        }
    }
}