﻿using System;

namespace InfiniteRoleplay.Entities
{
    public class Prisao
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Preso { get; set; } = 0;
        public int Policial { get; set; } = 0;
        public int Tempo { get; set; } = 0;
        public int Cela { get; set; } = 0;
    }
}