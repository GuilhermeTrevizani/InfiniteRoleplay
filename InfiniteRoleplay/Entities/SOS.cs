using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class SOS
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Mensagem { get; set; } = string.Empty;
        public int Usuario { get; set; } = 0;
        public DateTime? DataResposta { get; set; } = null;
        public int UsuarioStaff { get; set; } = 0;
        public bool Aceito { get; set; } = false;

        [NotMapped]
        public int IDPersonagem { get; set; }
    }
}