using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Commands : Script
    {
        [Command("ajuda", "!{#febd0c}USO:~w~ /ajuda")]
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
                new Comando("Geral", "/emtrabalho"),
                new Comando("Geral", "/emprego"),
                new Comando("Geral", "/staff", "Lista os membros da staff que estão online"),
                new Comando("Propriedades", "/entrar"),
                new Comando("Propriedades", "/sair"),
                new Comando("Propriedades", "/ptrancar"),
                new Comando("Propriedades", "/pcomprar"),
                new Comando("Propriedades", "/pvender"),
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
                new Comando("Celular", "/cel"),
                new Comando("Celular", "/gps"),
                new Comando("Veículos", "/vcomprar"),
                new Comando("Veículos", "/motor"),
                new Comando("Veículos", "/vtrancar"),
                new Comando("Veículos", "/vcomprarvaga"),
                new Comando("Veículos", "/vestacionar"),
                new Comando("Veículos", "/vspawn"),
                new Comando("Veículos", "/vlista"),
                new Comando("Veículos", "/vvender"),
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

            if (p.Emprego > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Emprego", "/sairemprego", "Sai do emprego"),
                });

                if (p.Emprego == (int)TipoEmprego.Taxista)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/taxiduty", "Entra ou sai de serviço como taxista"),
                        new Comando("Emprego", "/taxicha", "Exibe as chamadas aguardando taxistas"),
                        new Comando("Emprego", "/taxiac", "Atende uma chamada de taxista"),
                    });
            }

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
                        new Comando("Facção Policial", "/algemar"),
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
                    new Comando("Helper", "/ir"),
                    new Comando("Helper", "/trazer"),
                    new Comando("Helper", "/tp"),
                    new Comando("Helper", "/vw"),
                    new Comando("Helper", "/o"),
                    new Comando("Helper", "/a"),
                    new Comando("Helper", "/kick"),
                    new Comando("Helper", "/irveh"),
                    new Comando("Helper", "/trazerveh"),
                });

            if (p.UsuarioBD.Staff >= 2)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Game Moderator", "/vida"),
                    new Comando("Game Moderator", "/colete"),
                    new Comando("Game Moderator", "/skina"),
                    new Comando("Game Moderator", "/skinc"),
                    new Comando("Game Moderator", "/checar"),
                    new Comando("Game Moderator", "/ban"),
                    new Comando("Game Moderator", "/unban"),
                    new Comando("Game Moderator", "/banoff"),
                });

            if (p.UsuarioBD.Staff >= 3)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Game Administrator", "/ck"),
                });

            if (p.UsuarioBD.Staff >= 1337)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Manager", "/gmx"),
                    new Comando("Manager", "/tempo"),
                    new Comando("Manager", "/proximo"),
                    new Comando("Manager", "/cblip"),
                    new Comando("Manager", "/rblip"),
                    new Comando("Manager", "/addwhite"),
                    new Comando("Manager", "/delwhite"),
                    new Comando("Manager", "/setstaff"),
                    new Comando("Manager", "/cfac"),
                    new Comando("Manager", "/efacnome"),
                    new Comando("Manager", "/efactipo"),
                    new Comando("Manager", "/efaccor"),
                    new Comando("Manager", "/efacrankgestor"),
                    new Comando("Manager", "/efacranklider"),
                    new Comando("Manager", "/rfac"),
                    new Comando("Manager", "/faccoes"),
                    new Comando("Manager", "/crank"),
                    new Comando("Manager", "/eranknome"),
                    new Comando("Manager", "/rrank"),
                    new Comando("Manager", "/ranks"),
                    new Comando("Manager", "/lider"),
                    new Comando("Manager", "/parametros"),
                    new Comando("Manager", "/cprop"),
                    new Comando("Manager", "/rprop"),
                    new Comando("Manager", "/epropvalor"),
                    new Comando("Manager", "/epropint"),
                    new Comando("Manager", "/eproppos"),
                    new Comando("Manager", "/irblip"),
                    new Comando("Manager", "/irprop"),
                    new Comando("Manager", "/cpreco"),
                    new Comando("Manager", "/rpreco"),
                    new Comando("Manager", "/cponto"),
                    new Comando("Manager", "/rponto"),
                    new Comando("Manager", "/irponto"),
                    new Comando("Manager", "/eranksalario"),
                    new Comando("Manager", "/carm"),
                    new Comando("Manager", "/earmpos"),
                    new Comando("Manager", "/earmfac"),
                    new Comando("Manager", "/rarm"),
                    new Comando("Manager", "/irarm"),
                    new Comando("Manager", "/carmi"),
                    new Comando("Manager", "/rarmi"),
                    new Comando("Manager", "/earmimun"),
                    new Comando("Manager", "/earminrank"),
                    new Comando("Manager", "/earminest"),
                    new Comando("Manager", "/irarm"),
                    new Comando("Manager", "/eblipinativo"),
                });

            NAPI.ClientEvent.TriggerClientEvent(player, "comandoAjuda", listaComandos.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToList());
        }

        [Command("stats", "!{#febd0c}USO:~w~ /stats")]
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

        [Command("id", "!{#febd0c}USO:~w~ /id (ID ou nome)", GreedyArg = true)]
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

        [Command("aceitar", "!{#febd0c}USO:~w~ /aceitar (tipo)", Alias = "ac")]
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

                    if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
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

                    if (player.Position.DistanceTo(new Vector3(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ)) > 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo da propriedade!");
                        return;
                    }

                    p.Dinheiro -= valor;
                    p.SetDinheiro();
                    target.Dinheiro += valor;
                    target.SetDinheiro();

                    prop.Personagem = p.Codigo;

                    using (var context = new RoleplayContext())
                    {
                        context.Propriedades.Update(prop);
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

                    if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
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
                case TipoConvite.VendaVeiculo:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono do veículo não está online!");
                        break;
                    }

                    if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono do veículo não está próximo de você!");
                        return;
                    }

                    int.TryParse(convite.Valor[0], out int veiculo);
                    int.TryParse(convite.Valor[1], out int valorVeh);

                    if (p.Dinheiro < valorVeh)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                        break;
                    }

                    var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);
                    if (veh == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Propriedade inválida!");
                        break;
                    }

                    if (player.Position.DistanceTo(veh.Vehicle.Position) > 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo do veículo!");
                        return;
                    }

                    p.Dinheiro -= valorVeh;
                    p.SetDinheiro();
                    target.Dinheiro += valorVeh;
                    target.SetDinheiro();

                    veh.Personagem = p.Codigo;

                    using (var context = new RoleplayContext())
                    {
                        context.Veiculos.Update(veh);
                        context.SaveChanges();
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou o veículo {veh.Codigo} de {target.NomeIC} por ${valorVeh:N0}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Você vendeu o veículo {veh.Codigo} para {p.NomeIC} por ${valorVeh:N0}.");
                    break;
            }

            p.Convites.RemoveAll(x => x.Tipo == tipo);
        }

        [Command("recusar", "!{#febd0c}USO:~w~ /recusar (tipo)", Alias = "rc")]
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
                case TipoConvite.VendaVeiculo:
                    strPlayer = "compra de veículo";
                    strTarget = "venda de veículo";
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

        [Command("pagar", "!{#febd0c}USO:~w~ /pagar (ID ou nome) (valor)")]
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

            if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
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

        [Command("revistar", "!{#febd0c}USO:~w~ /revistar (ID ou nome)")]
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

            if (player.Position.DistanceTo(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
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

        [Command("multas", "!{#febd0c}USO:~w~ /multas")]
        public void CMD_multas(Client player) => Functions.VisualizarMultas(player, string.Empty);

        [Command("transferir", "!{#febd0c}USO:~w~ /transferir (ID ou nome) (valor)")]
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

        [Command("sacar", "!{#febd0c}USO:~w~ /sacar (valor)")]
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

        [Command("depositar", "!{#febd0c}USO:~w~ /depositar (valor)")]
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

        [Command("comprar", "!{#febd0c}USO:~w~ /comprar")]
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

        [Command("skin", "!{#febd0c}USO:~w~ /skin (skin)")]
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

        [Command("emtrabalho", "!{#febd0c}USO:~w~ /emtrabalho")]
        public void CMD_emtrabalho(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Jogadores trabalhando");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Policiais: {Global.PersonagensOnline.Count(x => x.FaccaoBD?.Tipo == (int)TipoFaccao.Policial && x.IsEmTrabalho)} | Médicos: {Global.PersonagensOnline.Count(x => x.FaccaoBD?.Tipo == (int)TipoFaccao.Medica && x.IsEmTrabalho)} | Taxistas: {Global.PersonagensOnline.Count(x => x.Emprego == (int)TipoEmprego.Taxista && x.IsEmTrabalho)}");
        }

        [Command("sairemprego", "!{#febd0c}USO:~w~ /sairemprego")]
        public void CMD_sairemprego(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não tem um emprego!");
                return;
            }

            p.Emprego = 0;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu do seu emprego!");
        }

        [Command("emprego", "!{#febd0c}USO:~w~ /emprego")]
        public void CMD_emprego(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já tem um emprego!");
                return;
            }

            if (p.FaccaoBD?.Tipo == (int)TipoFaccao.Policial || p.FaccaoBD?.Tipo == (int)TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode pegar um emprego pois está em uma facção governamental!");
                return;
            }

            var emprego = TipoEmprego.Nenhum;
            foreach (var c in Global.Empregos)
            {
                if (emprego == TipoEmprego.Nenhum && player.Position.DistanceTo(c.Posicao) <= 2)
                    emprego = c.Tipo;
            }

            if (emprego == TipoEmprego.Nenhum)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum local de emprego!");
                return;
            }

            p.Emprego = (int)emprego;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pegou o emprego {Functions.ObterDisplayEnum(emprego)}!");
        }

        [Command("staff", "!{#febd0c}USO:~w~ /staff")]
        public void CMD_staff(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.UsuarioBD?.Staff > 0).OrderByDescending(x => x.UsuarioBD.Staff).ThenBy(x => x.UsuarioBD.Nome).ToList();
            if (players.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhum membro da staff online!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Infinite Roleplay | Staff Online");
            foreach (var pl in players)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.UsuarioBD.NomeStaff} {pl.UsuarioBD.Nome}");
        }
    }
}