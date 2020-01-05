using System;

namespace InfiniteRoleplay.Entities
{
    public class Log
    {
        public long Codigo { get; set; }
        public DateTime Data { get; set; }
        public int Tipo { get; set; }
        public string Descricao { get; set; }
        public int PersonagemOrigem { get; set; }
        public int PersonagemDestino { get; set; }
        public string SocialClubOrigem { get; set; }
        public string SocialClubDestino { get; set; }
        public string IPOrigem { get; set; }
        public string IPDestino { get; set; }
    }
}