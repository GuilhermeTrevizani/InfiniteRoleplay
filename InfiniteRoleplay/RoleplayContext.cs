using InfiniteRoleplay.Entities;
using Microsoft.EntityFrameworkCore;

namespace InfiniteRoleplay
{
    public class RoleplayContext : DbContext
    {
        public DbSet<Armario> Armarios { get; set; }
        public DbSet<ArmarioItem> ArmariosItens { get; set; }
        public DbSet<Banimento> Banimentos { get; set; }
        public DbSet<Blip> Blips { get; set; }
        public DbSet<Faccao> Faccoes { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Multa> Multas { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Personagem> Personagens { get; set; }
        public DbSet<PersonagemAcessorio> PersonagensAcessorios { get; set; }
        public DbSet<PersonagemArma> PersonagensArmas { get; set; }
        public DbSet<PersonagemContato> PersonagensContatos { get; set; }
        public DbSet<PersonagemRoupa> PersonagensRoupas { get; set; }
        public DbSet<Ponto> Pontos { get; set; }
        public DbSet<Preco> Precos { get; set; }
        public DbSet<Prisao> Prisoes { get; set; }
        public DbSet<Propriedade> Propriedades { get; set; }
        public DbSet<Punicao> Punicoes { get; set; }
        public DbSet<Rank> Ranks { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Whitelist> Whitelist { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(Global.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Armario>().HasKey(x => x.Codigo);
            modelBuilder.Entity<ArmarioItem>().HasKey(x => new { x.Codigo, x.Arma });
            modelBuilder.Entity<Banimento>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Blip>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Faccao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Log>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Multa>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Parametro>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Personagem>().HasKey(x => x.Codigo);
            modelBuilder.Entity<PersonagemAcessorio>().HasKey(x => new { x.Codigo, x.Slot, x.Drawable, x.Texture });
            modelBuilder.Entity<PersonagemArma>().HasKey(x => new { x.Codigo, x.Arma });
            modelBuilder.Entity<PersonagemContato>().HasKey(x => new { x.Codigo, x.Celular });
            modelBuilder.Entity<PersonagemRoupa>().HasKey(x => new { x.Codigo, x.Slot, x.Drawable, x.Texture });
            modelBuilder.Entity<Ponto>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Preco>().HasKey(x => new { x.Tipo, x.Nome });
            modelBuilder.Entity<Prisao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Propriedade>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Punicao>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Rank>().HasKey(x => new { x.Faccao, x.Codigo });
            modelBuilder.Entity<Usuario>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Veiculo>().HasKey(x => x.Codigo);
            modelBuilder.Entity<Whitelist>().HasKey(x => x.SocialClub);
        }
    }
}