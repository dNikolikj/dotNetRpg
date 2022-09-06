using Microsoft.EntityFrameworkCore;

namespace dotNetRpg.Data
{
    public class DataContext : DbContext
    {


        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>()
                .HasData
                (
                new Skill()
                {
                    Id = 1,
                    Name = "FireBall", Damage = 30
                },
                new Skill()
                {
                    Id = 2,
                    Name = "Frenzy",
                    Damage = 20
                },
                new Skill()
                {
                    Id = 3,
                    Name= "Blizzard ",
                    Damage = 50
                }
                );
        }

        public DbSet<Character> Characters => Set<Character>(); // to escape the warning
        public DbSet<User> Users => Set<User>(); // to escape the warning
        public DbSet<Weapon> Weapons => Set<Weapon>(); // to escape the warning
        public DbSet<Skill> Skills => Set<Skill>(); // to escape the warning


    }
}
