using System;

namespace InfiniteRoleplay.Entities
{
    public class Whitelist
    {
        public string SocialClub { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public int Usuario { get; set; } = 0;
    }
}