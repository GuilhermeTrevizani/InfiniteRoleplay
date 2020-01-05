using InfiniteRoleplay.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfiniteRoleplay
{
    public class RoleplayContext : DbContext
    {
        public DbSet<Banimento> Banimentos { get; set; }
        public DbSet<Blip> Blips { get; set; }
        public DbSet<Faccao> Faccoes { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<PersonagemAcessorio> PersonagensAcessorios { get; set; }
        public DbSet<PersonagemArma> PersonagensArmas { get; set; }
        public DbSet<PersonagemRoupa> PersonagensRoupas { get; set; }
        public DbSet<Punicao> Punicoes { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Whitelist> Whitelist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql($"Server=localhost;Database=bdinfiniteroleplay;Uid=root;Pwd=");
            optionsBuilder.UseMySql($"Server=localhost;Database=bdinfiniteroleplay;Uid=root;Pwd=NdVawtSLq95u");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Banimento>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Blip>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Faccao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Log>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Personagem>().HasKey(x => x.Codigo);
            modelBuilder.Entity<PersonagemAcessorio>().HasKey(x => new { x.Codigo, x.Slot, x.Drawable, x.Texture });
            modelBuilder.Entity<PersonagemArma>().HasKey(x => new { x.Codigo, x.Arma });
            modelBuilder.Entity<PersonagemRoupa>().HasKey(x => new { x.Codigo, x.Slot, x.Drawable, x.Texture });
            modelBuilder.Entity<Punicao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Rank>().HasKey(x => new { x.Faccao, x.Codigo });
            modelBuilder.Entity<Usuario>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Whitelist>().HasKey(x => x.SocialClub);
        }
    }
}