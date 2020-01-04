using GTANetworkAPI;
using InfiniteRoleplay.Entities;
using System;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Commands : Script
    {
        [Command("ajuda")]
        public void CMD_ajuda(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Infinite Roleplay");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "GERAL: /reg /log /cper /per /stopanim (/sa) /stats /id");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "CHAT: /me /do /g /b /baixo /s /pm");

            if (p.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO: /f /membros");

                if (p.Rank >= p.FaccaoBD.RankGestor)
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO GESTOR: /blockf");

                if (p.Rank >= p.FaccaoBD.RankLider)
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO LÍDER: /crank /erank /rrank /ranks");
            }

            if (p.UsuarioBD.Staff >= 1)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1: /ir /trazer /tp /vw /o /a");

            if (p.UsuarioBD.Staff >= 2)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 2: /vida /colete /skin /skina /skinc");

            if (p.UsuarioBD.Staff >= 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1337: /gmx /tempo /proximo /cblip /rblip /addwhite /delwhite /staff");
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1337: /cfac /efac /rfac /faccoes /crank /erank /rrank /ranks /fac");
            }
        }

        [Command("reg")]
        public void CMD_reg(Client player, string usuario, string email, string senha)
        {
            var p = Functions.ObterPersonagem(player);
            if (p != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já está conectado!");
                return;
            }

            if (usuario.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Usuário não pode possuir mais que 25 caracteres!");
                return;
            }

            if (email.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Email não pode possuir mais que 100 caracteres!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Usuarios.Any(x => x.Nome == usuario))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Usuário {usuario} já existe!");
                    return;
                }

                if (context.Usuarios.Any(x => x.Email == email))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Email {email} já está sendo utilizado!");
                    return;
                }

                var user = new Usuario()
                {
                    Nome = usuario,
                    Email = email,
                    Senha = Functions.Criptografar(senha),
                    SocialClub = player.SocialClubName,
                    Serial = player.Serial,
                    IPRegistro = player.Address,
                    DataRegistro = DateTime.Now,
                    IPUltimoAcesso = player.Address,
                    DataUltimoAcesso = DateTime.Now,
                };
                context.Usuarios.Add(user);
                context.SaveChanges();

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Usuário {usuario} criado com sucesso!");
                CMD_log(player, usuario, senha);
            }
        }

        [Command("log")]
        public void CMD_log(Client player, string usuario, string senha)
        {
            var per = Functions.ObterPersonagem(player);
            if (per != null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já está conectado!");
                return;
            }

            var senhaCriptografada = Functions.Criptografar(senha);
            using (var context = new RoleplayContext())
            {
                var user = context.Usuarios.FirstOrDefault(x => x.Nome == usuario && x.Senha == senhaCriptografada);
                if (user == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Usuário ou senha inválidos!");
                    return;
                }

                user.DataUltimoAcesso = DateTime.Now;
                user.IPRegistro = player.Address;
                context.Usuarios.Update(user);
                context.SaveChanges();

                Global.PersonagensOnline.Add(new Personagem()
                {
                    UsuarioBD = user,
                });

                var personagens = context.Personagens.Where(x => x.Usuario == user.Codigo).ToList();
                if (personagens.Count == 0)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui personagens! Use /cper para criar um.");
                }
                else
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "Seus Personagens");
                    foreach (var p in personagens)
                        Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{p.Nome} [{p.Codigo}]");
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "Use /per (id) para logar no personagem ou /cper para criar um.");
                }
            }
        }

        [Command("cper")]
        public void CMD_cper(Client player, string nome, string sobrenome, string sexo, string dataNascimento)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.ID > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já está conectado!");
                return;
            }

            var nomeCompleto = $"{nome} {sobrenome}";
            if (nomeCompleto.Length > 25)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Nome do personagem não pode possuir mais que 25 caracteres!");
                return;
            }

            sexo = sexo.ToUpper();
            if (sexo != "F" && sexo != "M")
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Sexo deve ser M ou F!");
                return;
            }

            DateTime.TryParse(dataNascimento, out DateTime dtNascimento);
            if (dtNascimento == DateTime.MinValue)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Data de Nascimento não foi informada corretamente!");
                return;
            }

            var dif = DateTime.Now.Date - dtNascimento;
            if (dif.Days / 365 < 18)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Personagem precisa ter 18 anos ou mais!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                if (context.Personagens.Any(x => x.Nome == nomeCompleto))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {nomeCompleto} já existe!");
                    return;
                }

                var personagem = new Personagem()
                {
                    Nome = nomeCompleto,
                    Usuario = p.UsuarioBD.Codigo,
                    Sexo = sexo,
                    DataNascimento = dtNascimento,
                    IPRegistro = player.Address,
                    DataRegistro = DateTime.Now,
                    IPUltimoAcesso = player.Address,
                    DataUltimoAcesso = DateTime.Now,
                    Skin = 188012277,
                    Vida = 100,
                    PosX = 342.675f,
                    PosY = -1398.45f,
                    PosZ = 32.5093f,
                    Online = true,
                    ID = Global.PersonagensOnline.Max(x => x.ID) + 1,
                };
                context.Personagens.Add(personagem);
                context.SaveChanges();

                var user = p.UsuarioBD;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = personagem;
                Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(personagem)].UsuarioBD = user;

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Personagem {nomeCompleto} criado com sucesso!");
                Functions.LogarPersonagem(player, personagem);
            }
        }

        [Command("per")]
        public void CMD_per(Client player, int id)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.ID > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já está conectado!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Codigo == id && x.Usuario == p.UsuarioBD.Codigo);
                if (personagem == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {id} não existe ou não pertence a você!");
                    return;
                }

                personagem.DataUltimoAcesso = DateTime.Now;
                personagem.IPRegistro = player.Address;
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

        [Command("stopanim", Alias = "sa")]
        public void CMD_stopanim(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            player.StopAnimation();
        }

        [Command("stats")]
        public void CMD_stats(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Informações de {p.Nome} [{p.Codigo}]");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"OOC: {p.UsuarioBD.Nome} | SocialClub: {player.SocialClubName} | Staff: {p.UsuarioBD.Staff}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Registro: {p.DataRegistro.ToString()} | Tempo Conectado: {p.TempoConectado}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Sexo: {p.Sexo} | Nascimento: {p.DataNascimento.ToShortDateString()}");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Skin: {((PedHash)player.Model).ToString()} | Vida: {player.Health} | Colete: {player.Armor}");

            if (p.Faccao > 0)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Facção: {p.FaccaoBD?.Nome} [{p.Faccao}] | Rank: {p.RankBD?.Nome} [{p.Rank}]");
        }

        [Command("id", GreedyArg = true)]
        public void CMD_id(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            int.TryParse(idNome, out int id);
            var personagens = Global.PersonagensOnline.Where(x => x.ID == id || x.Nome.ToLower().Contains(idNome.ToLower())).OrderBy(x => x.ID).ToList();
            if (personagens.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum jogador foi encontrado com a pesquisa: {idNome}");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Jogadores encontrados com a pesquisa: {idNome}");
            foreach (var pl in personagens)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.Nome} [{pl.ID}] ({pl.UsuarioBD.Nome})");
        }
    }
}