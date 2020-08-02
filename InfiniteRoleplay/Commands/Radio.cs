using GTANetworkAPI;

namespace InfiniteRoleplay.Commands
{
    public class Radio : Script
    {
        [Command("canal", "!{#febd0c}USO:~w~ /canal (slot [1-3]) (canal)")]
        public void CMD_canal(Player player, int slot, int canal)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.CanalRadio == -1)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui um rádio!");
                return;
            }

            if (p.TempoPrisao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está preso!");
                return;
            }

            if (slot < 1 || slot > 3)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Slot deve ser entre 1 e 3!");
                return;
            }

            if (canal == 911 && p.FaccaoBD?.Tipo != TipoFaccao.Policial)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 911 é reservado para facções policiais!");
                return;
            }

            if (canal == 912 && p.FaccaoBD?.Tipo != TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Canal 912 é reservado para facções médicas!");
                return;
            }

            if (slot == 1)
                p.CanalRadio = canal;
            else if (slot == 2)
                p.CanalRadio2 = canal;
            else if (slot == 3)
                p.CanalRadio3 = canal;

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou seu canal de rádio do slot {slot} para {canal}!");
        }

        [Command("r", "!{#febd0c}USO:~w~ /r (mensagem)", GreedyArg = true)]
        public void CMD_r(Player player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 1, mensagem);

        [Command("r2", "!{#febd0c}USO:~w~ /r2 (mensagem)", GreedyArg = true)]
        public void CMD_r2(Player player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 2, mensagem);

        [Command("r3", "!{#febd0c}USO:~w~ /r3 (mensagem)", GreedyArg = true)]
        public void CMD_r3(Player player, string mensagem) => Functions.EnviarMensagemRadio(Functions.ObterPersonagem(player), 3, mensagem);
    }
}