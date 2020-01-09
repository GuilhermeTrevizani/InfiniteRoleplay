﻿using GTANetworkAPI;
using System.ComponentModel.DataAnnotations.Schema;

namespace InfiniteRoleplay.Entities
{
    public class Propriedade
    {
        public int Codigo { get; set; }
        public int Valor { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public float EntradaPosX { get; set; } = 0;
        public float EntradaPosY { get; set; } = 0;
        public float EntradaPosZ { get; set; } = 0;
        public float SaidaPosX { get; set; } = 0;
        public float SaidaPosY { get; set; } = 0;
        public float SaidaPosZ { get; set; } = 0;

        [NotMapped]
        public TextLabel TextLabel { get; set; }

        [NotMapped]
        public Marker Marker { get; set; }

        [NotMapped]
        public bool IsAberta { get; set; } = false;

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            TextLabel = NAPI.TextLabel.CreateTextLabel($"Nº {Codigo} {(Personagem == 0 ? $"(${Valor.ToString("N0")})" : string.Empty)}", new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ), 5, 2, 0, new Color(255, 255, 255));

            var corMarker = Personagem > 0 ? new Color(255, 106, 77) : new Color(110, 180, 105);
            Marker = NAPI.Marker.CreateMarker(MarkerType.UpsideDownCone, new Vector3(EntradaPosX, EntradaPosY, EntradaPosZ), new Vector3(), new Vector3(), 0.5f, corMarker);
        }

        public void DeletarIdentificador()
        {
            TextLabel?.Delete();
            Marker?.Delete();
        }
    }
}