using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System;
using System.Collections.Generic;
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
                return;

            var listaComandos = new List<Comando>()
            {
                new Comando("Teclas", "F2", "Jogadores online"),
                new Comando("Teclas", "F3", "Mostrar/ocultar cursor"),
                new Comando("Geral", "/stopanim /sa"),
                new Comando("Geral", "/stats"),
                new Comando("Geral", "/id"),
                new Comando("Geral", "/aceitar /ac"),
                new Comando("Geral", "/recusar /rc"),
                new Comando("Geral", "/pagar"),
                new Comando("Geral", "/trocarpersonagem"),
                new Comando("Geral", "/entrar"),
                new Comando("Geral", "/sair"),
                new Comando("Geral", "/p"),
                new Comando("Chat IC", "/me"),
                new Comando("Chat IC", "/do"),
                new Comando("Chat IC", "/g"),
                new Comando("Chat IC", "/baixo"),
                new Comando("Chat IC", "/s"),
                new Comando("Chat OOC", "/b"),
                new Comando("Chat OOC", "/pm"),
            };

            if (p.Faccao > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Facção", "/f"),
                    new Comando("Facção", "/membros"),
                });

                if (p.FaccaoBD.Tipo == (int)TipoFaccao.Policial)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Policial", "/m"),
                    });

                if (p.Rank >= p.FaccaoBD.RankGestor)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Gestor", "/blockf"),
                        new Comando("Facção Gestor", "/convidar"),
                        new Comando("Facção Gestor", "/rank"),
                        new Comando("Facção Gestor", "/demitir"),
                    });

                if (p.Rank >= p.FaccaoBD.RankLider)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Líder", "/crank"),
                        new Comando("Facção Líder", "/erank"),
                        new Comando("Facção Líder", "/rrank"),
                        new Comando("Facção Líder", "/ranks"),
                    });
            }

            if (p.UsuarioBD.Staff >= 1)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Staff 1", "/ir"),
                    new Comando("Staff 1", "/trazer"),
                    new Comando("Staff 1", "/tp"),
                    new Comando("Staff 1", "/vw"),
                    new Comando("Staff 1", "/o"),
                    new Comando("Staff 1", "/a"),
                    new Comando("Staff 1", "/kick"),
                });

            if (p.UsuarioBD.Staff >= 2)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Staff 2", "/vida"),
                    new Comando("Staff 2", "/colete"),
                    new Comando("Staff 2", "/skin"),
                    new Comando("Staff 2", "/skina"),
                    new Comando("Staff 2", "/skinc"),
                    new Comando("Staff 2", "/checar"),
                    new Comando("Staff 2", "/ban"),
                    new Comando("Staff 2", "/unban"),
                    new Comando("Staff 2", "/banoff"),
                });

            if (p.UsuarioBD.Staff >= 1337)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Staff 1337", "/gmx"),
                    new Comando("Staff 1337", "/tempo"),
                    new Comando("Staff 1337", "/proximo"),
                    new Comando("Staff 1337", "/cblip"),
                    new Comando("Staff 1337", "/rblip"),
                    new Comando("Staff 1337", "/addwhite"),
                    new Comando("Staff 1337", "/delwhite"),
                    new Comando("Staff 1337", "/staff"),
                    new Comando("Staff 1337", "/cfac"),
                    new Comando("Staff 1337", "/efac"),
                    new Comando("Staff 1337", "/rfac"),
                    new Comando("Staff 1337", "/faccoes"),
                    new Comando("Staff 1337", "/crank"),
                    new Comando("Staff 1337", "/erank"),
                    new Comando("Staff 1337", "/rrank"),
                    new Comando("Staff 1337", "/ranks"),
                    new Comando("Staff 1337", "/lider"),
                    new Comando("Staff 1337", "/parametros"),
                    new Comando("Staff 1337", "/dinheiro"),
                    new Comando("Staff 1337", "/cprop"),
                    new Comando("Staff 1337", "/rprop"),
                    new Comando("Staff 1337", "/eprop"),
                });

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoAjuda", listaComandos.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToList());
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

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);

            switch ((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    int.TryParse(convite.Valor[0], out int faccao);
                    int.TryParse(convite.Valor[1], out int rank);
                    p.Faccao = faccao;
                    p.Rank = rank;

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou o convite para entrar na facção.");
                    if (target != null)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} aceitou seu convite para entrar na facção.");
                    break;
                case TipoConvite.VendaPropriedade:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono da propriedade não está online!");
                        break;
                    }

                    float distance = player.Position.DistanceTo(target.Player.Position);
                    if (distance > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono da propriedade não está próximo de você!");
                        return;
                    }

                    int.TryParse(convite.Valor[0], out int propriedade);
                    int.TryParse(convite.Valor[1], out int valor);
                    if (p.Dinheiro < valor)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                        break;
                    }

                    var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == propriedade);
                    if (prop == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Propriedade inválida!");
                        break;
                    }

                    distance = player.Position.DistanceTo(new Vector3(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ));
                    if (distance > 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo da propriedade!");
                        return;
                    }

                    p.Dinheiro -= valor;
                    p.SetDinheiro();
                    target.Dinheiro += valor;
                    target.SetDinheiro();

                    Global.Propriedades[Global.Propriedades.IndexOf(prop)].Personagem = p.Codigo;

                    using (var context = new RoleplayContext())
                    {
                        context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prop)]);
                        context.SaveChanges();
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a propriedade {prop.Codigo} de {Functions.ObterNomeIC(target)} por ${valor.ToString("N0")}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Você vendeu a propriedade {prop.Codigo} para {Functions.ObterNomeIC(p)} por ${valor.ToString("N0")}.");
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

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);
            var strPlayer = string.Empty;
            var strTarget = string.Empty;

            switch ((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    strPlayer = strTarget = "entrar na facção";
                    break;
                case TipoConvite.VendaPropriedade:
                    strPlayer = "compra da propriedade";
                    strTarget = "venda da propriedade";
                    break;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você recusou o convite para {strPlayer}.");

            if (target != null)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} recusou seu convite para {strTarget}.");

            p.Convites.RemoveAll(x => x.Tipo == tipo);
        }

        [Command("pagar")]
        public void CMD_pagar(Client player, string idNome, int dinheiro)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.Dinheiro < dinheiro)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            float distance = player.Position.DistanceTo(target.Player.Position);
            if (distance > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            p.Dinheiro -= dinheiro;
            target.Dinheiro += dinheiro;
            p.SetDinheiro();
            target.SetDinheiro();

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{Functions.ObterNomeIC(p)} te deu ${dinheiro.ToString("N0")}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu ${dinheiro.ToString("N0")} para {Functions.ObterNomeIC(target)}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/pagar {dinheiro}", p, target);
        }

        [Command("trocarpersonagem")]
        public void CMD_trocarpersonagem(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.SalvarPersonagem(player, false);

            Global.PersonagensOnline[Global.PersonagensOnline.IndexOf(p)] = new Entities.Personagem()
            {
                UsuarioBD = p.UsuarioBD,
            };
            RemoteEvents.EVENT_voltarSelecionarPersonagem(player, true);
        }
    }
}