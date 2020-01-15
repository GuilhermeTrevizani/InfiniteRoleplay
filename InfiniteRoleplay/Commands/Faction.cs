using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Faction : Script
    {
        [Command("f", GreedyArg = true)]
        public void CMD_f(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            if (p.FaccaoBD.IsChatBloqueado)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Chat da facção está bloqueado!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#" + p.FaccaoBD.Cor + "}" + $"(( {p.RankBD.Nome} {p.Nome} [{p.ID}]: {mensagem} ))");
        }

        [Command("membros")]
        public void CMD_membros(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao).OrderByDescending(x => x.Rank).ThenBy(x => x.Nome);
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, "!{#" + p.FaccaoBD.Cor + "}" + p.FaccaoBD.Nome);
            foreach (var pl in players)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.RankBD.Nome} {pl.Nome} [{pl.ID}] (( {pl.UsuarioBD.Nome} ))");
        }

        [Command("blockf")]
        public void CMD_blockf(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            Global.Faccoes[Global.Faccoes.IndexOf(p.FaccaoBD)].IsChatBloqueado = !p.FaccaoBD.IsChatBloqueado;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(!p.FaccaoBD.IsChatBloqueado ? "des" : string.Empty)}bloqueou o chat da facção!");
        }

        [Command("convidar")]
        public void CMD_convidar(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador já está em uma facção!");
                return;
            }

            var rank = Global.Ranks.Where(x => x.Faccao == p.Faccao).Min(x => x.Codigo);
            var convite = new Convite()
            {
                Tipo = (int)TipoConvite.Faccao,
                Personagem = p.Codigo,
                Valor = new string[] { p.Faccao.ToString(), rank.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == (int)TipoConvite.Faccao);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você convidou {target.Nome} para a facção.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} convidou você para a facção {p.FaccaoBD.Nome}. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.FaccaoGestor, "/convidar", p, target);
        }

        [Command("rank")]
        public void CMD_rank(Client player, string idNome, int rank)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção!");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu!");
                return;
            }

            var rk = Global.Ranks.FirstOrDefault(x => x.Faccao == p.Faccao && x.Codigo == rank);
            if (rk == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Rank {rank} não existe!");
                return;
            }

            if (rank >= p.FaccaoBD.RankGestor && p.Rank < p.FaccaoBD.RankLider)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Somente o líder da facção pode alterar o rank de um jogador para gestor!");
                return;
            }

            target.Rank = rank;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou o rank de {target.Nome} para {rk.Nome} ({rk.Codigo}).");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} alterou seu rank para {rk.Nome} ({rk.Codigo}).");

            Functions.GravarLog(TipoLog.FaccaoGestor, $"/rank {rank}", p, target);
        }

        [Command("demitir")]
        public void CMD_demitir(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.Faccao != p.Faccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador não pertence a sua facção!");
                return;
            }

            if (target.Rank > p.Rank)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Jogador possui um rank maior que o seu!");
                return;
            }

            target.Faccao = 0;
            target.Rank = 0;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você demitiu {target.Nome} da facção.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.UsuarioBD.Nome} demitiu você da facção.");

            Functions.GravarLog(TipoLog.FaccaoGestor, "/demitir", p, target);
        }

        [Command("m", GreedyArg = true)]
        public void CMD_m(Client player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Megafone, 55.0f);
        }

        [Command("duty")]
        public void CMD_duty(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial && p?.FaccaoBD?.Tipo != (int)TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou médica!");
                return;
            }

            p.IsTrabalhoFaccao = !p.IsTrabalhoFaccao;
            foreach (var pl in Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao))
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#" + p.FaccaoBD.Cor + "}" + $"{p.RankBD.Nome} {p.Nome} {(p.IsTrabalhoFaccao ? "entrou em" : "saiu de")} serviço!");
        }

        [Command("sairfaccao")]
        public void CMD_sairfaccao(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            p.Faccao = p.Rank = 0;
            p.IsTrabalhoFaccao = false;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu da facção.");
        }

        [Command("multar", GreedyArg = true)]
        public void CMD_multar(Client player, string idNome, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsTrabalhoFaccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres!");
                return;
            }

            using (var context = new RoleplayContext())
            {
                context.Multas.Update(new Entities.Multa()
                {
                    Motivo = motivo,
                    PersonagemMultado = target.Codigo,
                    PersonagemPolicial = p.Codigo,
                    Valor = valor,
                });
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {target.Nome} por ${valor:N0}. Motivo: {motivo}");
            if (target.Celular > 0)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, "!{#F0E90D}" + $"[CELULAR] SMS de {target.ObterNomeContato(911)}: Você recebeu uma multa de ${valor:N0}. Motivo: {motivo}");
        }

        [Command("multaroff", GreedyArg = true)]
        public void CMD_multaroff(Client player, string nomeCompleto, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsTrabalhoFaccao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (motivo.Length > 255)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Motivo não pode ter mais que 255 caracteres!");
                return;
            }

            nomeCompleto = nomeCompleto.Replace("_", " ");

            using (var context = new RoleplayContext())
            {
                var personagem = context.Personagens.FirstOrDefault(x => x.Nome.ToLower() == nomeCompleto.ToLower() && !x.Online);
                if (personagem == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {nomeCompleto} não encontrado ou está online!");
                    return;
                }

                context.Multas.Update(new Entities.Multa()
                {
                    Motivo = motivo,
                    PersonagemMultado = personagem.Codigo,
                    PersonagemPolicial = p.Codigo,
                    Valor = valor,
                });
                context.SaveChanges();

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {personagem.Nome} por ${valor:N0}. Motivo: {motivo}");
            }
        }
    }
}