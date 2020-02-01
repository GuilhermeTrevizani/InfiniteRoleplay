using GTANetworkAPI;

namespace InfiniteRoleplay.Models
{
    public class Concessionaria
    {
        public string Nome { get; set; }
        public TipoPreco Tipo { get; set; }
        public Vector3 PosicaoCompra { get; set; }
        public Vector3 PosicaoSpawn { get; set; }
        public Vector3 RotacaoSpawn { get; set; }
    }
}