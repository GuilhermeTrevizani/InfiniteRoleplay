using GTANetworkAPI;
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
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "TECLAS: F2 (jogadores online)");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "GERAL: /stopanim (/sa) /stats /id /aceitar (/ac) /recusar (/rc)");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "CHAT: /me /do /g /b /baixo /s /pm");

            if (p.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO: /f /membros");

                if (p.Rank >= p.FaccaoBD.RankGestor)
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO GESTOR: /blockf /convidar /rank /demitir");

                if (p.Rank >= p.FaccaoBD.RankLider)
                    Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "FACÇÃO LÍDER: /crank /erank /rrank /ranks");
            }

            if (p.UsuarioBD.Staff >= 1)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1: /ir /trazer /tp /vw /o /a /kick");

            if (p.UsuarioBD.Staff >= 2)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 2: /vida /colete /skin /skina /skinc /checar /ban /unban /banoff");

            if (p.UsuarioBD.Staff >= 1337)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1337: /gmx /tempo /proximo /cblip /rblip /addwhite /delwhite /staff");
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "STAFF 1337: /cfac /efac /rfac /faccoes /crank /erank /rrank /ranks /lider");
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

            Functions.MostrarStats(player, p);
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

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Jogadores encontrados com a pesquisa: {idNome}");
            foreach (var pl in personagens)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.Nome} [{pl.ID}] ({pl.UsuarioBD.Nome})");
        }

        [Command("aceitar", Alias = "ac")]
        public void CMD_aceitar(Client player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoConvite), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            var convite = p.Convites.FirstOrDefault(x => x.Tipo == tipo);
            if (convite == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui nenhum convite do tipo {tipo}!");
                return;
            }

            switch((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    int.TryParse(convite.Valor[0], out int faccao);
                    int.TryParse(convite.Valor[1], out int rank);
                    p.Faccao = faccao;
                    p.Rank = rank;

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou o convite para entrar na facção.");
                    var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);
                    if (target != null)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} aceitou seu convite para entrar na facção.");
                    break;
            }

            p.Convites.RemoveAll(x => x.Tipo == tipo);
        }

        [Command("recusar", Alias = "rc")]
        public void CMD_recusar(Client player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoConvite), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            var convite = p.Convites.FirstOrDefault(x => x.Tipo == tipo);
            if (convite == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui nenhum convite do tipo {tipo}!");
                return;
            }

            switch ((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você recusou o convite para entrar na facção.");
                    var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);
                    if (target != null)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} recusou seu convite para entrar na facção.");
                    break;
            }

            p.Convites.RemoveAll(x => x.Tipo == tipo);
        }
    }
}