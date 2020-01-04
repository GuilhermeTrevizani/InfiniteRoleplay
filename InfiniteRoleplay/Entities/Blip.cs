namespace InfiniteRoleplay.Entities
{
    public class Blip
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public int Tipo { get; set; } = 0;
        public int Cor { get; set; } = 0;
    }
}