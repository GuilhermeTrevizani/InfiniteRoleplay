using GTANetworkAPI;
using System.Linq;

namespace InfiniteRoleplay
{
    public class RemoteEvents : Script
    {
        [RemoteEvent("playersOnline")]
        public void EVENT_playersOnlie(Client player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var personagens = Global.PersonagensOnline.Where(x => x.ID > 0).OrderBy(x => x.ID).ToList();
            Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Jogadores Online ({personagens.Count})");
            foreach (var pl in personagens)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.Nome} [{pl.ID}] ({pl.UsuarioBD.Nome}) | Ping: {pl.Player.Ping}");
        }
    }
}