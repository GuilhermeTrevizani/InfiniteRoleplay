﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Usuario
    {
        public int Codigo { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string SocialClubRegistro { get; set; } = string.Empty;
        public string SocialClubUltimoAcesso { get; set; } = string.Empty;
        public string IPRegistro { get; set; } = string.Empty;
        public DateTime DataRegistro { get; set; } = DateTime.Now;
        public string IPUltimoAcesso { get; set; } = string.Empty;
        public DateTime DataUltimoAcesso { get; set; } = DateTime.Now;
        public string Serial { get; set; } = string.Empty;
        public int Staff { get; set; } = 0;
        public bool PossuiNamechange { get; set; } = false;
        public int QuantidadeSOSAceitos { get; set; } = 0;
        public int TempoTrabalhoAdministrativo { get; set; } = 0;

        [NotMapped]
        public string NomeStaff
        {
            get
            {
                return Staff switch
                {
                    1337 => "Manager",
                    100 => "Lead Administrator",
                    3 => "Game Administrator",
                    2 => "Game Moderator",
                    1 => "Helper",
                    _ => Staff.ToString(),
                };
            }
        }
    }
}