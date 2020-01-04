namespace InfiniteRoleplay.Entities
{
    public class PersonagemArma
    {
        public int Codigo { get; set; }
        public string Arma { get; set; }
        public int Municao { get; set; } = 0;
        public int Pintura { get; set; } = 0;
        public string Componentes { get; set; } = string.Empty;
    }
}