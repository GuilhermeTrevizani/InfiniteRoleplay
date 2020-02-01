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
        public static List<Armario> Armarios { get; set; }
        public static List<ArmarioItem> ArmariosItens { get; set; }
        public static List<Concessionaria> Concessionarias { get; set; }
    }
}