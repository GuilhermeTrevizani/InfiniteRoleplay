using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Properties : Script
    {
        [Command("entrar", "!{#febd0c}USO:~w~ /entrar")]
        public void CMD_entrar(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma entrada!");
                return;
            }

            if (!prox.IsAberta)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "A porta está trancada!");
                return;
            }

            p.IPLs = Functions.ObterIPLsPorInterior((TipoInterior)prox.Interior);
            p.SetarIPLs();
            player.Dimension = (uint)prox.Codigo;
            player.Position = new Vector3(prox.SaidaPosX, prox.SaidaPosY, prox.SaidaPosZ);
        }

        [Command("sair", "!{#febd0c}USO:~w~ /sair")]
        public void CMD_sair(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Dimension == x.Codigo
                    && player.Position.DistanceTo(new Vector3(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma saída!");
                return;
            }

            if (!prox.IsAberta)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "A porta está trancada!");
                return;
            }

            p.LimparIPLs();
            player.Dimension = 0;
            player.Position = new Vector3(prox.EntradaPosX, prox.EntradaPosY, prox.EntradaPosZ);
        }

        [Command("ptrancar", "!{#febd0c}USO:~w~ /ptrancar")]
        public void CMD_ptrancar(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                prox = Global.Propriedades
                .Where(x => x.Codigo == player.Dimension
                && player.Position.DistanceTo(new Vector3(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.SaidaPosX, x.SaidaPosY, x.SaidaPosZ)))
                .FirstOrDefault();

                if (prox == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade!");
                    return;
                }
            }

            if (prox.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o dono da propriedade!");
                return;
            }

            Global.Propriedades[Global.Propriedades.IndexOf(prox)].IsAberta = !prox.IsAberta;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(Global.Propriedades[Global.Propriedades.IndexOf(prox)].IsAberta ? "des" : string.Empty)}trancou a porta!");
        }

        [Command("pcomprar", "!{#febd0c}USO:~w~ /pcomprar")]
        public void CMD_pcomprar(Player player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => x.Personagem == 0 && player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade à venda!");
                return;
            }

            if (p.Dinheiro < prox.Valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            p.Dinheiro -= prox.Valor;
            Global.Propriedades[Global.Propriedades.IndexOf(prox)].Personagem = p.Codigo;

            p.SetDinheiro();
            prox.CriarIdentificador();

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prox)]);
                context.SaveChanges();
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a propriedade por ${prox.Valor:N0}!");
        }

        [Command("pvender", "!{#febd0c}USO:~w~ /pvender (ID ou nome) (valor)")]
        public void CMD_pvender(Player player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => x.Personagem == p.Codigo && player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= 2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade sua!");
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

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor não é válido!");
                return;
            }

            var convite = new Convite()
            {
                Tipo = (int)TipoConvite.VendaPropriedade,
                Personagem = p.Codigo,
                Valor = new string[] { prox.Codigo.ToString(), valor.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == (int)TipoConvite.VendaPropriedade);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você ofereceu sua propriedade {prox.Codigo} para {target.NomeIC} por ${valor:N0}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} ofereceu para você a propriedade {prox.Codigo} por ${valor:N0}. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.Venda, $"/pvender {prox.Codigo} {valor}", p, target);
        }
    }
}