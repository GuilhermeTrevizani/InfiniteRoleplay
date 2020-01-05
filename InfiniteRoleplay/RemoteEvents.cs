using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using System;
using System.Globalization;
using System.Linq;

namespace InfiniteRoleplay
{
    public class RemoteEvents : Script
    {
        [RemoteEvent("playersOnline")]
        public void EVENT_playersOnline(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var personagens = Global.PersonagensOnline.Where(x => x.ID > 0).OrderBy(x => x.ID).Select(x => new { id = x.ID, nome = x.Nome, ping = x.Player.Ping }).ToList();
            NAPI.ClientEvent.TriggerClientEvent(player, "playersOnline", personagens);
        }

        [RemoteEvent("registrarUsuario")]
        public void EVENT_registrarUsuario(Client player, string usuario, string email, string senha, string senha2)
        {
            var p = Functions.ObterPersonagem(player);
            if (p != null)
                return;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha) || string.IsNullOrWhiteSpace(senha2))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            if (usuario.Length > 25)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Usuário não pode ter mais que 25 caracteres!");
                return;
            }

            if (email.Length > 100)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Email não pode ter mais que 100 caracteres!");
                return;
            }

            if (senha != senha2)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, "Senhas não estão iguais!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var uSocial = context.Usuarios.FirstOrDefault(x => x.SocialClubRegistro == player.SocialClubName);
                if (uSocial != null)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", uSocial.Nome, $"Você já possui o usuário {uSocial.Nome}!");
                    return;
                }

                if (context.Usuarios.Any(x => x.Nome == usuario))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, $"Usuário {usuario} já existe!");
                    return;
                }

                if (context.Usuarios.Any(x => x.Email == email))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "registrarUsuario", usuario, email, $"Email {email} já está sendo utilizado!");
                    return;
                }

                var user = new Usuario()
                {
                    Nome = usuario,
                    Email = email,
                    Senha = Functions.Criptografar(senha),
                    SocialClubRegistro = player.SocialClubName,
                    SocialClubUltimoAcesso = player.SocialClubName,
                    Serial = player.Serial,
                    IPRegistro = player.Address,
                    DataRegistro = DateTime.Now,
                    IPUltimoAcesso = player.Address,
                    DataUltimoAcesso = DateTime.Now,
                };
                context.Usuarios.Add(user);
                context.SaveChanges();

                EVENT_entrarUsuario(player, usuario, senha);
            }
        }

        [RemoteEvent("entrarUsuario")]
        public void EVENT_entrarUsuario(Client player, string usuario, string senha)
        {
            var p = Functions.ObterPersonagem(player);
            if (p != null)
                return;

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(senha))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", usuario, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            var senhaCriptografada = Functions.Criptografar(senha);
            using (var context = new RoleplayContext())
            {
                var user = context.Usuarios.FirstOrDefault(x => x.Nome == usuario && x.Senha == senhaCriptografada);
                if (user == null)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "playerConnected", usuario, "Usuário ou senha inválidos!");
                    return;
                }

                var banimento = context.Banimentos.FirstOrDefault(x => x.Usuario == user.Codigo);
                if (!Functions.VerificarBanimento(player, banimento))
                    return;

                user.DataUltimoAcesso = DateTime.Now;
                user.IPUltimoAcesso = player.Address;
                user.SocialClubUltimoAcesso = player.SocialClubName;
                context.Usuarios.Update(user);
                context.SaveChanges();

                Global.PersonagensOnline.Add(new Personagem()
                {
                    UsuarioBD = user,
                });

                EVENT_voltarSelecionarPersonagem(player, "");
            }
        }

        [RemoteEvent("criarPersonagem")]
        public void EVENT_criarPersonagem(Client player, string nome, string sobrenome, string sexo, string dataNascimento)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.ID > 0)
                return;

            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome) || string.IsNullOrWhiteSpace(sexo) || string.IsNullOrWhiteSpace(dataNascimento))
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, "Verifique se todos os campos foram preenchidos corretamente!");
                return;
            }

            nome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nome);
            sobrenome = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sobrenome);
            var nomeCompleto = $"{nome} {sobrenome}";
            if (nomeCompleto.Length > 25)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, "Nome do personagem não pode possuir mais que 25 caracteres!");
                return;
            }

            sexo = sexo.ToUpper();
            if (sexo != "F" && sexo != "M")
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, "Sexo deve ser M ou F!");
                return;
            }

            DateTime.TryParse(dataNascimento, out DateTime dtNascimento);
            if (dtNascimento == DateTime.MinValue)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, "Data de Nascimento não foi informada corretamente!");
                return;
            }

            var dif = DateTime.Now.Date - dtNascimento;
            if (dif.Days / 365 < 18)
            {
                NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, "Personagem precisa ter 18 anos ou mais!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Personagens.Any(x => x.Nome == nomeCompleto))
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "criarPersonagem", nome, sobrenome, sexo, dataNascimento, $"Personagem {nomeCompleto} já existe!");
                    return;
                }

                var personagem = new Personagem()
                {
                    Nome = nomeCompleto,
                    Usuario = p.UsuarioBD.Codigo,
                    Sexo = sexo,
                    DataNascimento = dtNascimento,
                    SocialClubRegistro = player.SocialClubName,
                    SocialClubUltimoAcesso = player.SocialClubName,
                    DataRegistro = DateTime.Now,
                    DataUltimoAcesso = DateTime.Now,
                    IPRegistro = player.Address,
                    IPUltimoAcesso = player.Address,
                    Skin = 188012277,
                    Vida = 100,
                    Colete = 0,
                    PosX = 128.4853f,
                    PosY = -1737.086f,
                    PosZ = 30.11018f,
                    Online = true,
                    ID = Global.PersonagensOnline.Max(x => x.ID) + 1,
                    Faccao = 0,
                    Rank = 0,
                    Dimensao = 0,
                    TempoConectado = 0,
                };
                context.Personagens.Add(personagem);
                context.SaveChanges();

                var user = p.UsuarioBD;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = personagem;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(personagem)].UsuarioBD = user;

                Functions.LogarPersonagem(player, personagem);
            }
        }

        [RemoteEvent("selecionarPersonagem")]
        public void EVENT_selecionarPersonagem(Client player, int id)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.ID > 0)
                return;

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == id && x.Usuario == p.UsuarioBD.Codigo);
                if (personagem == null)
                {
                    EVENT_voltarSelecionarPersonagem(player, $"Personagem {id} não existe ou não pertence a você!");
                    return;
                }

                personagem.DataUltimoAcesso = DateTime.Now;
                personagem.IPUltimoAcesso = player.Address;
                personagem.SocialClubUltimoAcesso = player.SocialClubName;
                personagem.ID = Global.PersonagensOnline.Max(x => x.ID) + 1;
                personagem.Online = true;
                context.Personagens.Update(personagem);
                context.SaveChanges();

                var user = p.UsuarioBD;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = personagem;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(personagem)].UsuarioBD = user;

                Functions.LogarPersonagem(player, personagem);
            }
        }

        [RemoteEvent("voltarSelecionarPersonagem")]
        public void EVENT_voltarSelecionarPersonagem(Client player, string erro)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            if (p.ID > 0)
                return;

            using (var context = new RoleplayContext())
            {
                var personagens = context.Personagens.Where(x => x.Usuario == p.UsuarioBD.Codigo).OrderBy(x => x.Codigo).Select(x => new { id = x.Codigo, nome = x.Nome }).ToList();
                NAPI.ClientEvent.TriggerClientEvent(player, "selecionarPersonagem", personagens, erro);
            }
        }
    }
}