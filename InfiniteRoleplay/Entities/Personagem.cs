using GTANetworkAPI;
using InfiniteRoleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace InfiniteRoleplay.Entities
{
    public class Personagem
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int Usuario { get; set; } = 0;
        public string SocialClubRegistro { get; set; } = string.Empty;
        public string SocialClubUltimoAcesso { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public string IPRegistro { get; set; } = string.Empty;
        public DateTime DataUltimoAcesso { get; set; } = DateTime.Now;
        public string IPUltimoAcesso { get; set; } = string.Empty;
        public string Skin { get; set; } = "FreeModeMale01";
        public double PosX { get; set; } = 128.4853f;
        public double PosY { get; set; } = -1737.086f;
        public double PosZ { get; set; } = 30.11018f;
        public int Vida { get; set; } = 100;
        public int Colete { get; set; } = 0;
        public long Dimensao { get; set; } = 0;
        public string Sexo { get; set; } = "M";
        public DateTime DataNascimento { get; set; } = DateTime.MinValue;
        public bool Online { get; set; } = true;
        public int TempoConectado { get; set; } = 0;
        public int Faccao { get; set; } = 0;
        public int Rank { get; set; } = 0;
        public int Dinheiro { get; set; } = 500;
        public int Celular { get; set; } = 0;
        public int Banco { get; set; } = 0;
        public string IPL { get; set; } = "[]";
        public int CanalRadio { get; set; } = -1;
        public int CanalRadio2 { get; set; } = 0;
        public int CanalRadio3 { get; set; } = 0;
        public int TempoPrisao { get; set; } = 0;
        public double RotX { get; set; } = 0;
        public double RotY { get; set; } = 0;
        public double RotZ { get; set; } = 149.1595f;
        public DateTime? DataMorte { get; set; } = null;
        public string MotivoMorte { get; set; } = string.Empty;
        public int Emprego { get; set; } = 0;

        [NotMapped]
        public int ID { get; set; }

        [NotMapped]
        public Usuario UsuarioBD { get; set; }

        [NotMapped]
        public Client Player { get => NAPI.Pools.GetAllPlayers().FirstOrDefault(x => x.SocialClubName == UsuarioBD?.SocialClubRegistro); }

        [NotMapped]
        public DateTime DataUltimaVerificacao { get; set; }

        [NotMapped]
        public Faccao FaccaoBD { get => Global.Faccoes.FirstOrDefault(x => x.Codigo == Faccao); }

        [NotMapped]
        public Rank RankBD { get => Global.Ranks.FirstOrDefault(x => x.Faccao == Faccao && x.Codigo == Rank); }

        [NotMapped]
        public List<Convite> Convites { get; set; }

        [NotMapped]
        public List<Propriedade> Propriedades { get => Global.Propriedades.Where(x => x.Personagem == Codigo).ToList(); }

        [NotMapped]
        public string NomeIC { get => Nome; }

        [NotMapped]
        public List<PersonagemContato> Contatos { get; set; }

        [NotMapped]
        public int NumeroLigacao { get; set; } = 0;

        [NotMapped]
        public int StatusLigacao { get; set; } = 0;

        [NotMapped]
        public string ExtraLigacao { get; set; } = string.Empty;

        [NotMapped]
        public TagTimer TimerCelular { get; set; }

        [NotMapped]
        public bool IsEmTrabalho { get; set; } = false;

        [NotMapped]
        public List<string> IPLs { get; set; }

        [NotMapped]
        public bool Algemado { get; set; } = false;

        [NotMapped]
        public int AguardandoTipoServico { get; set; } = 0;

        public void SetDinheiro()
        {
            if (Player != null)
                NAPI.ClientEvent.TriggerClientEvent(Player, "setDinheiro", Dinheiro.ToString("N0"));
        }

        public string ObterNomeContato(int numero)
        {
            if (numero == 911)
                return "Central de Emergência";

            if (numero == 5555555)
                return "Downtown Cab Company";

            var contato = Contatos.FirstOrDefault(x => x.Celular == numero);
            return contato == null ? $"#{numero}" : $"{contato.Nome} #{numero}";
        }

        public void LimparLigacao(bool isApenasPararTimer = false)
        {
            TimerCelular?.Stop();
            TimerCelular = null;

            if (!isApenasPararTimer)
            {
                NumeroLigacao = 0;
                StatusLigacao = 0;
                ExtraLigacao = string.Empty;
            }
        }

        public void SetarIPLs()
        {
            foreach (var ipl in IPLs)
                NAPI.ClientEvent.TriggerClientEvent(Player, "setIPL", ipl);
        }

        public void LimparIPLs()
        {
            foreach (var ipl in IPLs)
                NAPI.ClientEvent.TriggerClientEvent(Player, "removeIPL", ipl);

            IPLs.Clear();
        }
    }
}