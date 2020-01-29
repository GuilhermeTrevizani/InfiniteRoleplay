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
                new Comando("Geral", "/stats"),
                new Comando("Geral", "/id"),
                new Comando("Geral", "/aceitar /ac"),
                new Comando("Geral", "/recusar /rc"),
                new Comando("Geral", "/pagar"),
                new Comando("Geral", "/revistar"),
                new Comando("Geral", "/multas"),
                new Comando("Geral", "/comprar"),
                new Comando("Geral", "/skin"),
                new Comando("Propriedades", "/entrar"),
                new Comando("Propriedades", "/sair"),
                new Comando("Propriedades", "/ptrancar"),
                new Comando("Propriedades", "/pcomprar"),
                new Comando("Propriedades", "/pvenderpara"),
                new Comando("Chat IC", "/me"),
                new Comando("Chat IC", "/do"),
                new Comando("Chat IC", "/g"),
                new Comando("Chat IC", "/baixo"),
                new Comando("Chat IC", "/s"),
                new Comando("Chat OOC", "/b"),
                new Comando("Chat OOC", "/pm"),
                new Comando("Celular", "/sms"),
                new Comando("Celular", "/desligar /des"),
                new Comando("Celular", "/ligar"),
                new Comando("Celular", "/atender"),
                new Comando("Celular", "/celular"),
                new Comando("Celular", "/gps"),
                new Comando("Veículos", "/vcomprar"),
                new Comando("Veículos", "/motor"),
                new Comando("Veículos", "/vtrancar"),
                new Comando("Veículos", "/vcomprarvaga"),
                new Comando("Veículos", "/vestacionar"),
                new Comando("Veículos", "/vspawn"),
                new Comando("Veículos", "/vlista"),
                new Comando("Banco", "/depositar"),
                new Comando("Banco", "/sacar"),
                new Comando("Banco", "/transferir"),
                new Comando("Animações", "/stopanim /sa"),
                new Comando("Animações", "/crossarms"),
                new Comando("Animações", "/handsup /hs"),
                new Comando("Animações", "/smoke"),
                new Comando("Animações", "/lean"),
                new Comando("Animações", "/police"),
                new Comando("Animações", "/incar"),
                new Comando("Animações", "/pushups"),
                new Comando("Animações", "/situps"),
                new Comando("Animações", "/blunt"),
                new Comando("Animações", "/afishing"),
                new Comando("Animações", "/acop"),
                new Comando("Animações", "/idle"),
                new Comando("Animações", "/barra"),
                new Comando("Animações", "/kneel"),
                new Comando("Animações", "/revistarc"),
                new Comando("Animações", "/ajoelhar"),
                new Comando("Animações", "/drink"),
                new Comando("Animações", "/morto"),
                new Comando("Animações", "/gsign"),
                new Comando("Animações", "/hurry"),
                new Comando("Animações", "/cair"),
                new Comando("Animações", "/wsup"),
                new Comando("Animações", "/render"),
                new Comando("Animações", "/mirar"),
                new Comando("Animações", "/sentar"),
                new Comando("Animações", "/dormir"),
                new Comando("Animações", "/pixar"),
                new Comando("Animações", "/sexo"),
                new Comando("Animações", "/jogado"),
                new Comando("Animações", "/reparando"),
                new Comando("Animações", "/luto"),
                new Comando("Animações", "/bar"),
                new Comando("Animações", "/necessidades"),
                new Comando("Animações", "/meth"),
                new Comando("Animações", "/mijar"),
                new Comando("Rádio", "/canal"),
                new Comando("Rádio", "/r"),
                new Comando("Rádio", "/r2"),
                new Comando("Rádio", "/r3"),
            };

            if (p.Faccao > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Facção", "/f"),
                    new Comando("Facção", "/membros"),
                    new Comando("Facção", "/sairfaccao"),
                });

                if (p.FaccaoBD.Tipo == (int)TipoFaccao.Policial)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Policial", "/m"),
                        new Comando("Facção Policial", "/duty"),
                        new Comando("Facção Policial", "/multar"),
                        new Comando("Facção Policial", "/multaroff"),
                        new Comando("Facção Policial", "/prender"),
                    });
                else if (p.FaccaoBD.Tipo == (int)TipoFaccao.Medica)

                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Médica", "/duty"),
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
                    new Comando("Staff 1", "/irveh"),
                    new Comando("Staff 1", "/trazerveh"),
                });

            if (p.UsuarioBD.Staff >= 2)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Staff 2", "/vida"),
                    new Comando("Staff 2", "/colete"),
                    new Comando("Staff 2", "/skina"),
                    new Comando("Staff 2", "/skinc"),
                    new Comando("Staff 2", "/checar"),
                    new Comando("Staff 2", "/ban"),
                    new Comando("Staff 2", "/unban"),
                    new Comando("Staff 2", "/banoff"),
                });

            if (p.UsuarioBD.Staff >= 3)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Staff 3", "/ck"),
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
                    new Comando("Staff 1337", "/efacnome"),
                    new Comando("Staff 1337", "/efactipo"),
                    new Comando("Staff 1337", "/efaccor"),
                    new Comando("Staff 1337", "/efacrankgestor"),
                    new Comando("Staff 1337", "/efacranklider"),
                    new Comando("Staff 1337", "/rfac"),
                    new Comando("Staff 1337", "/faccoes"),
                    new Comando("Staff 1337", "/crank"),
                    new Comando("Staff 1337", "/eranknome"),
                    new Comando("Staff 1337", "/rrank"),
                    new Comando("Staff 1337", "/ranks"),
                    new Comando("Staff 1337", "/lider"),
                    new Comando("Staff 1337", "/parametros"),
                    new Comando("Staff 1337", "/cprop"),
                    new Comando("Staff 1337", "/rprop"),
                    new Comando("Staff 1337", "/epropvalor"),
                    new Comando("Staff 1337", "/epropint"),
                    new Comando("Staff 1337", "/eproppos"),
                    new Comando("Staff 1337", "/irblip"),
                    new Comando("Staff 1337", "/irprop"),
                    new Comando("Staff 1337", "/cpreco"),
                    new Comando("Staff 1337", "/rpreco"),
                    new Comando("Staff 1337", "/cponto"),
                    new Comando("Staff 1337", "/rponto"),
                    new Comando("Staff 1337", "/irponto"),
                    new Comando("Staff 1337", "/eranksalario"),
                    new Comando("Staff 1337", "/carm"),
                    new Comando("Staff 1337", "/earmpos"),
                    new Comando("Staff 1337", "/earmfac"),
                    new Comando("Staff 1337", "/rarm"),
                    new Comando("Staff 1337", "/irarm"),
                    new Comando("Staff 1337", "/carmi"),
                    new Comando("Staff 1337", "/rarmi"),
                    new Comando("Staff 1337", "/earmimun"),
                    new Comando("Staff 1337", "/earminrank"),
                    new Comando("Staff 1337", "/earminest"),
                    new Comando("Staff 1337", "/irarm"),
                });

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoAjuda", listaComandos.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToList());
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

                    var distance = player.Position.DistanceTo(target.Player.Position);
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

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a propriedade {prop.Codigo} de {target.NomeIC} por ${valor:N0}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Você vendeu a propriedade {prop.Codigo} para {p.NomeIC} por ${valor:N0}.");
                    break;
                case TipoConvite.Revista:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Solicitante da revista não está online!");
                        break;
                    }

                    float dist = player.Position.DistanceTo(target.Player.Position);
                    if (dist > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Solicitante da revista não está próximo de você!");
                        return;
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou ser revistado.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Titulo, $"Revista em {p.NomeIC}");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"Celular: {p.Celular} | Dinheiro: ${p.Dinheiro:N0}");
                    if (p.CanalRadio > -1)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"Canal Rádio 1: {p.CanalRadio} | Canal Rádio 2: {p.CanalRadio2} | Canal Rádio 3: {p.CanalRadio3}");
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
                case TipoConvite.Revista:
                    strPlayer = strTarget = "revista";
                    break;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você recusou o convite para {strPlayer}.");

            if (target != null)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} recusou seu convite para {strTarget}.");

            p.Convites.RemoveAll(x => x.Tipo == tipo);
        }

        [Command("pagar")]
        public void CMD_pagar(Client player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (p.Dinheiro < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            float distance = player.Position.DistanceTo(target.Player.Position);
            if (distance > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            p.Dinheiro -= valor;
            target.Dinheiro += valor;
            p.SetDinheiro();
            target.SetDinheiro();

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} te deu ${valor:N0}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu ${valor:N0} para {target.NomeIC}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/pagar {valor}", p, target);
        }

        [Command("revistar")]
        public void CMD_revistar(Client player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            float distance = player.Position.DistanceTo(target.Player.Position);
            if (distance > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            var convite = new Convite()
            {
                Tipo = (int)TipoConvite.Revista,
                Personagem = p.Codigo,
            };
            target.Convites.RemoveAll(x => x.Tipo == (int)TipoConvite.Faccao);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você solicitou uma revista para {target.Nome}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Solicitou uma revista em você. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");
        }

        [Command("multas")]
        public void CMD_multas(Client player) => Functions.VisualizarMultas(player, string.Empty);

        [Command("transferir")]
        public void CMD_transferir(Client player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.TempoPrisao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está preso!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => (x.Tipo == (int)TipoPonto.Banco || x.Tipo == (int)TipoPonto.ATM) && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2) && p.Celular == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM ou não possui um celular!");
                return;
            }

            if (p.Banco < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente no banco!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            p.Banco -= valor;
            target.Banco += valor;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} transferiu para você ${valor:N0}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você transferiu ${valor:N0} para {target.Nome}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/transferir {valor}", p, target);
        }

        [Command("sacar")]
        public void CMD_sacar(Client player, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => (x.Tipo == (int)TipoPonto.Banco || x.Tipo == (int)TipoPonto.ATM) && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM!");
                return;
            }

            if (p.Banco < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente no banco!");
                return;
            }

            p.Banco -= valor;
            p.Dinheiro += valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você sacou ${valor:N0}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/sacar {valor}", p, null);
        }

        [Command("depositar")]
        public void CMD_depositar(Client player, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == (int)TipoPonto.Banco && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco!");
                return;
            }

            if (p.Dinheiro < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            p.Dinheiro -= valor;
            p.Banco += valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você depositou ${valor:N0}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/depositar {valor}", p, null);
        }

        [Command("comprar")]
        public void CMD_comprar(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == (int)TipoPonto.LojaConveniencia && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma loja de conveniência!");
                return;
            }

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoComprar", Global.Precos.Where(x => x.Tipo == (int)TipoPreco.Conveniencia).OrderBy(x => x.Nome).Select(x => new
            {
                x.Nome,
                Preco = $"${x.Valor:N0}",
            }).ToList(), 0, string.Empty);
        }

        [Command("skin")]
        public void CMD_skin(Client player, string skin)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == (int)TipoPonto.LojaRoupas && player.Position.DistanceTo(new Vector3(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma loja de roupas!");
                return;
            }

            var pedHash = NAPI.Util.PedNameToModel(skin);
            if (pedHash == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Skin {skin} não existe!");
                return;
            }

            if (!Global.Skins.Any(x => x.Nome == pedHash.ToString() && !x.IsBloqueada && x.TipoFaccao == null && x.Sexo == p.Sexo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Skin bloqueada ou indisponível para o sexo do seu personagem!");
                return;
            }

            if (p.Dinheiro < Global.Parametros.ValorSkin)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            p.Dinheiro -= Global.Parametros.ValorSkin;
            p.SetDinheiro();
            player.SetSkin(pedHash);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a skin {pedHash.ToString()} por ${Global.Parametros.ValorSkin:N0}.");
        }
    }
}