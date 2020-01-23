namespace InfiniteRoleplay.Entities
{
    public class ArmarioItem
    {
        public int Codigo { get; set; }
        public string Arma { get; set; }
        public int Municao { get; set; } = 1;
        public int Estoque { get; set; } = 0;
        public int Pintura { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public string Componentes { get; set; } = "[]";
    }
}