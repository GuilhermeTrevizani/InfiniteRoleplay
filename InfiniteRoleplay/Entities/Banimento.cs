using System;

namespace InfiniteRoleplay.Entities
{
    public class Banimento
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; }
        public DateTime? Expiracao { get; set; }
        public int Usuario { get; set; }
        public string SocialClub { get; set; }
        public string Motivo { get; set; }
        public int UsuarioStaff { get; set; }
    }
}