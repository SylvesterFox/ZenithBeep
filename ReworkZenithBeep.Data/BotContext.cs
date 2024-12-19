using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ReworkZenithBeep.Data.Models.items;


namespace ReworkZenithBeep.Data
{
    public class BotContext : DbContext
    {
        public BotContext(DbContextOptions<BotContext> options) : base(options) { }


        public DbSet<ItemGuild> Guilds { get; set; }
        public DbSet<ItemRolesSelector> Roles { get; set; }

        public DbSet<ItemRoomersLobby> RoomersLobbies { get; set; }

        public DbSet<ItemRooomsSettings> ItemsRooms { get; set; }

        public DbSet<ItemTempRoom> ItemsTempRooms { get; set; }

        public DbSet<ItemUser> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemGuild>()
                .HasMany(e => e.Roles)
                .WithOne(e => e.Guild)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemGuild>()
                .HasMany(e => e.RoomersLobby)
                .WithOne(e => e.Guild)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemRolesSelector>()
                .HasOne(e => e.Guild)
                .WithMany(e => e.Roles)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemUser>()
                .HasMany(e => e.Roomers)
                .WithOne(e => e.User)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemRooomsSettings>()
                .HasOne(e => e.User)
                .WithMany(e => e.Roomers)
                .HasForeignKey(e => e.Id)
                .IsRequired();

            modelBuilder.Entity<ItemTempRoom>()
                .HasKey(e => e.roomid);

        }
    }

    public class BotContextFactory : IDesignTimeDbContextFactory<BotContext>
    {
        public BotContext CreateDbContext(string[] atgs)
        {
            var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
            var dataConfig = DataConfig.InitDataConfig();
            optionsBuilder.UseNpgsql(dataConfig, x => x.MigrationsAssembly("ReworkZenithBeep.Data.Migrations"));
            return new BotContext(optionsBuilder.Options);
        }
    }
}
