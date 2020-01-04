using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Faccao
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public int Tipo { get; set; } = 0;
        public string Cor { get; set; } = string.Empty;
        public int RankGestor { get; set; } = 0;
        public int RankLider { get; set; } = 0;

        [NotMapped]
        public bool ChatBloqueado { get; set; } = false;
    }
}