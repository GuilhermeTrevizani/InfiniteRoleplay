using System;

namespace InfiniteRoleplay.Entities
{
    public class Punicao
    {
        public int Codigo { get; set; }
        public int Tipo { get; set; }
        public int Duracao { get; set; }
        public DateTime Data { get; set; }
        public int Personagem { get; set; }
        public string Motivo { get; set; }
        public int UsuarioStaff { get; set; }
    }
}