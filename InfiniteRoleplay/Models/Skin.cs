namespace InfiniteRoleplay.Models
{
    public class Skin
    {
        public Skin(string nome, bool isBloqueada)
        {
            Nome = nome;
            IsBloqueada = isBloqueada;
        }

        public Skin(string nome, string sexo)
        {
            Nome = nome;
            Sexo = sexo;
        }

        public string Nome { get; set; }
        public string Sexo { get; set; }
        public bool IsBloqueada { get; set; }
    }
}