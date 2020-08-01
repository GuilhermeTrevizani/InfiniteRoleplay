using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System.Linq;
using static InfiniteRoleplay.Constants;

namespace InfiniteRoleplay.Commands
{
    public class Faction : Script
    {
        [Command("f", "!{#febd0c}USO:~w~ /f (mensagem)", GreedyArg = true)]
        public void CMD_f(Player player, string mensagem)
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

        [Command("membros", "!{#febd0c}USO:~w~ /membros")]
        public void CMD_membros(Player player)
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

        [Command("blockf", "!{#febd0c}USO:~w~ /blockf")]
        public void CMD_blockf(Player player)
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

        [Command("convidar", "!{#febd0c}USO:~w~ /convidar (ID ou nome)")]
        public void CMD_convidar(Player player, string idNome)
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

        [Command("rank", "!{#febd0c}USO:~w~ /rank (ID ou nome) (rank)")]
        public void CMD_rank(Player player, string idNome, int rank)
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

        [Command("demitir", "!{#febd0c}USO:~w~ /demitir (ID ou nome)")]
        public void CMD_demitir(Player player, string idNome)
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

        [Command("m", "!{#febd0c}USO:~w~ /m (mensagem)", GreedyArg = true)]
        public void CMD_m(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, mensagem, TipoMensagemJogo.Megafone, 55.0f);
        }

        [Command("duty", "!{#febd0c}USO:~w~ /duty")]
        public void CMD_duty(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial && p?.FaccaoBD?.Tipo != (int)TipoFaccao.Medica && p?.Emprego == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou médica e não possui um emprego!");
                return;
            }

            p.IsEmTrabalho = !p.IsEmTrabalho;

            if (p?.Faccao == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(p.IsEmTrabalho ? "entrou em" : "saiu de")} serviço!");
            }
            else
            {
                foreach (var pl in Global.PersonagensOnline.Where(x => x.Faccao == p.Faccao))
                    Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#" + p.FaccaoBD.Cor + "}" + $"{p.RankBD.Nome} {p.Nome} {(p.IsEmTrabalho ? "entrou em" : "saiu de")} serviço!");
            }
        }

        [Command("sairfaccao", "!{#febd0c}USO:~w~ /sairfaccao")]
        public void CMD_sairfaccao(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção!");
                return;
            }

            p.Faccao = p.Rank = 0;
            p.IsEmTrabalho = false;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu da facção.");
        }

        [Command("multar", "!{#febd0c}USO:~w~ /multar (ID ou nome) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multar(Player player, string idNome, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsEmTrabalho)
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
                context.Multas.Add(new Entities.Multa()
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

        [Command("multaroff", "!{#febd0c}USO:~w~ /multar (nome completo) (valor) (motivo)", GreedyArg = true)]
        public void CMD_multaroff(Player player, string nomeCompleto, int valor, string motivo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsEmTrabalho)
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

            using var context = new RoleplayContext();
            var personagem = context.Personagens.FirstOrDefault(x => x.Nome.ToLower() == nomeCompleto.ToLower() && !x.Online);
            if (personagem == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Personagem {nomeCompleto} não encontrado ou está online!");
                return;
            }

            context.Multas.Add(new Entities.Multa()
            {
                Motivo = motivo,
                PersonagemMultado = personagem.Codigo,
                PersonagemPolicial = p.Codigo,
                Valor = valor,
            });
            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você multou {personagem.Nome} por ${valor:N0}. Motivo: {motivo}");
        }

        [Command("prender", "!{#febd0c}USO:~w~ /prender (ID ou nome) (cela [1-3]) (minutos)")]
        public void CMD_prender(Player player, string idNome, int cela, int minutos)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            if (player.Position.DistanceTo(PosicaoPrisao) > 2)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no local que as prisões são efetuadas!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (target.TempoPrisao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador já está preso!");
                return;
            }

            if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            if (minutos <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Minutos inválidos!");
                return;
            }

            var pos = new Vector3();
            switch (cela)
            {
                case 1:
                    pos = new Vector3(460.4085, -994.0992, 25); 
                    break;
                case 2:
                    pos = new Vector3(460.4085, -997.7994, 25);
                    break;
                case 3:
                    pos = new Vector3(460.4085, -1001.342, 25);
                    break;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Cela deve ser entre 1 e 3!");
                    break;
            }

            using (var context = new RoleplayContext())
            {
                context.Prisoes.Add(new Entities.Prisao()
                {
                    Preso = target.Codigo,
                    Policial = p.Codigo,
                    Tempo = minutos,
                    Cela = cela,
                });
                context.SaveChanges();
            }

            target.Player.Position = pos;
            target.TempoPrisao = minutos;
            Functions.EnviarMensagemTipoFaccao(TipoFaccao.Policial, $"{p.RankBD.Nome} {p.Nome} prendeu {target.Nome} na cela {cela} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.", true, true);
            Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"{p.Nome} prendeu você na cela {cela} por {minutos} minuto{(minutos > 1 ? "s" : string.Empty)}.");
        }

        [Command("algemar", "!{#febd0c}USO:~w~ /algemar (ID ou nome)")]
        public void CMD_algemar(Player player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.FaccaoBD?.Tipo != (int)TipoFaccao.Policial || !p.IsEmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma facção policial ou não está em serviço!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            target.Algemado = !target.Algemado;
            target.Player.SetSharedData("IsAnimacao", target.Algemado);

            if (p.Algemado)
            {
                target.Player.PlayAnimation("mp_arresting", "idle", (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl));

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você algemou {target.NomeIC}.");
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} algemou você.");
            }
            else
            {
                target.Player.StopAnimation();

                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você desalgemou {target.NomeIC}.");
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} desalgemou você.");
            }
        }

        [Command("gov", "!{#febd0c}USO:~w~ /gov (mensagem)", GreedyArg = true)]
        public void CMD_gov(Player player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.Faccao == 0 || p?.Rank == 0 || p?.Rank < p?.FaccaoBD?.RankGestor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            foreach (var pl in Global.PersonagensOnline.Where(x => x.Codigo > 0))
            {
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, "!{#" + p.FaccaoBD.Cor + "}" + p.FaccaoBD.Nome);
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem);
            }
        }
    }
}