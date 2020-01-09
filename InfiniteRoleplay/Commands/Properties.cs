using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System.Linq;

namespace InfiniteRoleplay.Commands
{
    public class Properties : Script
    {
        [Command("entrar")]
        public void CMD_entrar(Client player)
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

            player.Dimension = (uint)prox.Codigo;
            player.Position = new Vector3(prox.SaidaPosX, prox.SaidaPosY, prox.SaidaPosZ);
        }

        [Command("sair")]
        public void CMD_sair(Client player)
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

            player.Dimension = 0;
            player.Position = new Vector3(prox.EntradaPosX, prox.EntradaPosY, prox.EntradaPosZ);
        }

        [Command("p")]
        public void CMD_p(Client player, string parametro = "", string idNome = "", int valor = 0)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var prox = Global.Propriedades
                .Where(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <=2)
                .OrderBy(x => player.Position.DistanceTo(new Vector3(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)))
                .FirstOrDefault();

            if (prox == null && parametro != "porta")
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhuma propriedade!");
                return;
            }

            switch (parametro)
            {
                case "porta":
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
                    return;
                case "comprar":
                    if (prox.Personagem != 0)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "A propriedade já tem dono!");
                        return;
                    }

                    if (p.Dinheiro < prox.Valor)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                        return;
                    }

                    p.Dinheiro -= prox.Valor;
                    Global.Propriedades[Global.Propriedades.IndexOf(prox)].Personagem = p.Codigo;
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a propriedade por ${prox.Valor.ToString("N0")}!");
                    break;
                case "vender":
                    if (prox.Personagem != p.Codigo)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o dono da propriedade!");
                        return;
                    }

                    var propValor = prox.Valor / 2;
                    p.Dinheiro += propValor;
                    Global.Propriedades[Global.Propriedades.IndexOf(prox)].Personagem = 0;
                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você vendeu a propriedade por ${propValor.ToString("N0")}!");
                    break;
                case "venderpara":
                    if (prox.Personagem != p.Codigo)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o dono da propriedade!");
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(idNome))
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não é válido!");
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

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você ofereceu sua propriedade {prox.Codigo} para {Functions.ObterNomeIC(target)} por ${valor.ToString("N0")}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{Functions.ObterNomeIC(p)} ofereceu para você a propriedade {prox.Codigo} ${valor.ToString("N0")}. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");

                    Functions.GravarLog(TipoLog.Venda, $"/p venderpara {valor}", p, target);
                    return;
                default:
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Parâmetro inválido. Utilize: porta, comprar, vender, venderpara");
                    return;
            }

            p.SetDinheiro();
            prox.CriarIdentificador();

            using (var context = new RoleplayContext())
            {
                context.Propriedades.Update(Global.Propriedades[Global.Propriedades.IndexOf(prox)]);
                context.SaveChanges();
            }
        }
    }
}