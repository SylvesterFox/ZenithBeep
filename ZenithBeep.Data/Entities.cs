using Microsoft.EntityFrameworkCore;

namespace ZenithBeep.Data;

public sealed class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<LobbyEntity> Lobbies => Set<LobbyEntity>();
    public DbSet<TemporaryRoomEntity> TemporaryRooms => Set<TemporaryRoomEntity>();
    public DbSet<RoleSelectorEntity> RoleSelectors => Set<RoleSelectorEntity>();
    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<LobbyEntity>().HasKey(x => x.GuildId);
        builder.Entity<TemporaryRoomEntity>().HasKey(x => x.ChannelId);
        builder.Entity<RoleSelectorEntity>().HasIndex(x => new { x.GuildId, x.MessageId, x.Emoji }).IsUnique();
    }
}
public sealed class LobbyEntity { public ulong GuildId { get; set; } public ulong ChannelId { get; set; } }
public sealed class TemporaryRoomEntity { public ulong ChannelId { get; set; } public ulong GuildId { get; set; } public ulong OwnerId { get; set; } }
public sealed class RoleSelectorEntity { public int Id { get; set; } public ulong GuildId { get; set; } public ulong MessageId { get; set; } public ulong ChannelId { get; set; } public ulong RoleId { get; set; } public string Emoji { get; set; } = string.Empty; }
