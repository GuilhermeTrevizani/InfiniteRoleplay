using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using Microsoft.EntityFrameworkCore;
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
            player.Name = $"{p.Nome} [{p.ID}]";
            player.Dimension = (uint)p.Dimensao;
            player.Position = new Vector3(p.PosX, p.PosY, p.PosZ);
            player.Health = p.Vida;
            player.Armor = p.Colete;
            player.SetSkin((uint)p.Skin);

            using (var context = new RoleplayContext())
            {
                var roupas = context.PersonagensRoupas.Where(x => x.Codigo == p.Codigo);
                foreach (var r in roupas)
                    player.SetClothes(r.Slot, r.Drawable, r.Texture);

                var acessorios = context.PersonagensAcessorios.Where(x => x.Codigo == p.Codigo);
                foreach (var a in acessorios)
                    player.SetAccessories(a.Slot, a.Drawable, a.Texture);
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "logarPersonagem", "Infinite Roleplay (infiniteroleplay.com.br)", p.Nome);
        }

        public static Personagem ObterPersonagem(Client player)
        {
            return Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClub == player.SocialClubName);
        }

        public static Personagem ObterPersonagemPorIdNome(Client player, string idNome)
        {
            int.TryParse(idNome, out int id);
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.ID == id);
            if (p != null)
                return p;

            var ps = Global.PersonagensOnline.Where(x => x.Nome.ToLower().Contains(idNome.ToLower())).ToList();
            if (ps.Count == 1)
                return ps.FirstOrDefault();

            if (ps.Count > 0)
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Mais de um personagem online foi encontrado com a pesquisa: {idNome}");
                foreach (var pl in ps)
                    EnviarMensagem(player, TipoMensagem.Nenhum, $"[ID: {pl.ID}] {pl.Nome}");
            }
            else
            {
                EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum personagem online foi encontrado com a pesquisa: {idNome}");
            }

            return null;
        }

        public static void SalvarPersonagem(Client player, bool isOnline = true)
        {
            var p = ObterPersonagem(player);
            if (p == null)
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
                context.Personagens.Update(personagem);

                context.Database.ExecuteSqlCommand($"DELETE FROM PersonagensAcessorios WHERE Codigo = {p.Codigo}");
                context.Database.ExecuteSqlCommand($"DELETE FROM PersonagensRoupas WHERE Codigo = {p.Codigo}");

                var acessorios = new List<PersonagemAcessorio>();
                var roupas = new List<PersonagemRoupa>();

                for (var i = 0; i <= 15; i++)
                {
                    var drawable = player.GetAccessoryDrawable(i);
                    var texture = player.GetAccessoryTexture(i);

                    if (drawable > 0 || texture > 0)
                        acessorios.Add(new PersonagemAcessorio()
                        {
                            Codigo = p.Codigo,
                            Slot = i,
                            Drawable = drawable,
                            Texture = texture,
                        });

                    drawable = player.GetClothesDrawable(i);
                    texture = player.GetClothesTexture(i);

                    if (drawable > 0 || texture > 0)
                        roupas.Add(new PersonagemRoupa()
                        {
                            Codigo = p.Codigo,
                            Slot = i,
                            Drawable = drawable,
                            Texture = texture,
                        });
                }

                if (acessorios.Count > 0)
                    context.PersonagensAcessorios.AddRange(acessorios);

                if (roupas.Count > 0)
                    context.PersonagensRoupas.AddRange(roupas);

                context.SaveChanges();
            }
        }

        public static void SendMessageToNearbyPlayers(Client player, string message, TipoMensagemJogo type, float range, bool excludePlayer = false)
        {
            float distanceGap = range / 5;

            List<Client> targetList = NAPI.Pools.GetAllPlayers().Where(p => Global.PersonagensOnline.Any(x => x.UsuarioBD.SocialClub == p.SocialClubName) && p.Dimension == player.Dimension).ToList();

            foreach (Client target in targetList)
            {
                if (player != target || (player == target && !excludePlayer))
                {
                    float distance = player.Position.DistanceTo(target.Position);

                    if (distance <= range)
                    {
                        string chatMessageColor = GetChatMessageColor(distance, distanceGap);

                        var p = Global.PersonagensOnline.FirstOrDefault(x => x.UsuarioBD.SocialClub == player.SocialClubName);
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
                                target.SendChatMessage(chatMessageColor + $"{ObterNomeIC(p)} diz baixo: {message}");
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
    }
}