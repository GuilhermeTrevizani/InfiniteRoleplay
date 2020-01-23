using InfiniteRoleplay.Entities;
using InfiniteRoleplay.Models;
using System.Collections.Generic;

namespace InfiniteRoleplay
{
    public static class Global
    {
        public static string ConnectionString { get; set; }
        public static Parametro Parametros { get; set; }
        public static List<Personagem> PersonagensOnline { get; set; }
        public static List<Blip> Blips { get; set; }
        public static List<Faccao> Faccoes { get; set; }
        public static List<Rank> Ranks { get; set; }
        public static List<Propriedade> Propriedades { get; set; }
        public static List<Preco> Precos { get; set; }
        public static List<Veiculo> Veiculos { get; set; }
        public static List<Ponto> Pontos { get; set; }
        public static List<Skin> Skins { get; set; }
        public static GTANetworkAPI.Vector3 PosicaoPrisao { get; set; } = new GTANetworkAPI.Vector3(461.7921, -989.0697, 24.91488);
        public static List<Armario> Armarios { get; set; }
        public static List<ArmarioItem> ArmariosItens { get; set; }
    }

    public class Constants
    {
        public const string COLOR_CHAT_CLOSE = "!{#E6E6E6}";
        public const string COLOR_CHAT_NEAR = "!{#C8C8C8}";
        public const string COLOR_CHAT_MEDIUM = "!{#AAAAAA}";
        public const string COLOR_CHAT_FAR = "!{#8C8C8C}";
        public const string COLOR_CHAT_LIMIT = "!{#6E6E6E}";

        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        };
    }
}