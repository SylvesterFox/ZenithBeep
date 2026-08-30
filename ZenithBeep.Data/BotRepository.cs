using Microsoft.EntityFrameworkCore;

namespace ZenithBeep.Data;

public sealed class BotRepository(IDbContextFactory<BotDbContext> factory)
{
    public async Task<ulong?> GetLobbyAsync(ulong guildId) { await using var db = await factory.CreateDbContextAsync(); return await db.Lobbies.Where(x => x.GuildId == guildId).Select(x => (ulong?)x.ChannelId).SingleOrDefaultAsync(); }
    public async Task SetLobbyAsync(ulong guildId, ulong channelId) { await using var db = await factory.CreateDbContextAsync(); db.Lobbies.Add(new LobbyEntity { GuildId = guildId, ChannelId = channelId }); await db.SaveChangesAsync(); }
    public async Task DeleteLobbyAsync(ulong guildId) { await using var db = await factory.CreateDbContextAsync(); await db.Lobbies.Where(x => x.GuildId == guildId).ExecuteDeleteAsync(); }
    public async Task AddTemporaryRoomAsync(ulong guildId, ulong channelId, ulong ownerId) { await using var db = await factory.CreateDbContextAsync(); db.TemporaryRooms.Add(new TemporaryRoomEntity { GuildId = guildId, ChannelId = channelId, OwnerId = ownerId }); await db.SaveChangesAsync(); }
    public async Task<bool> IsTemporaryRoomAsync(ulong channelId) { await using var db = await factory.CreateDbContextAsync(); return await db.TemporaryRooms.AnyAsync(x => x.ChannelId == channelId); }
    public async Task DeleteTemporaryRoomAsync(ulong channelId) { await using var db = await factory.CreateDbContextAsync(); await db.TemporaryRooms.Where(x => x.ChannelId == channelId).ExecuteDeleteAsync(); }
    public async Task<RoleSelectorEntity?> GetRoleSelectorAsync(ulong guildId, ulong messageId, ulong channelId, string emoji) { await using var db = await factory.CreateDbContextAsync(); return await db.RoleSelectors.SingleOrDefaultAsync(x => x.GuildId == guildId && x.MessageId == messageId && x.ChannelId == channelId && x.Emoji == emoji); }
    /// <summary>Creates a selector or returns null when this message already uses the same emoji.</summary>
    public async Task<RoleSelectorEntity?> AddRoleSelectorAsync(RoleSelectorEntity item)
    {
        await using var db = await factory.CreateDbContextAsync();
        var alreadyExists = await db.RoleSelectors.AnyAsync(x =>
            x.GuildId == item.GuildId && x.MessageId == item.MessageId && x.Emoji == item.Emoji);
        if (alreadyExists) return null;

        db.RoleSelectors.Add(item);
        try
        {
            await db.SaveChangesAsync();
            return item;
        }
        catch (DbUpdateException)
        {
            // A second command can pass the check concurrently; the database remains authoritative.
            return null;
        }
    }
    public async Task<bool> DeleteRoleSelectorAsync(ulong guildId, int id) { await using var db = await factory.CreateDbContextAsync(); return await db.RoleSelectors.Where(x => x.GuildId == guildId && x.Id == id).ExecuteDeleteAsync() > 0; }
    public async Task<List<RoleSelectorEntity>> ListRoleSelectorsAsync(ulong guildId, ulong? messageId = null) { await using var db = await factory.CreateDbContextAsync(); var q = db.RoleSelectors.Where(x => x.GuildId == guildId); if (messageId is not null) q = q.Where(x => x.MessageId == messageId); return await q.OrderBy(x => x.Id).ToListAsync(); }
}
