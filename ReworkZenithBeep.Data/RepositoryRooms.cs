
using DSharpPlus.Entities;
using Microsoft.EntityFrameworkCore;
using ReworkZenithBeep.Data.Models.items;

namespace ReworkZenithBeep.Data
{
    public class RepositoryRooms : DataBot
    {

        private readonly IDbContextFactory<BotContext> _contextFactory;
        public RepositoryRooms(IDbContextFactory<BotContext> contextFactory) : base(contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<bool> CreateLobbyData(DiscordChannel channel)
        {
            ItemGuild itemGuild = await GetOrCreateGuild(channel.Guild);
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == itemGuild.Id);
            if (await query.AnyAsync() == false)
            {
                var lobbyData = new ItemRoomersLobby()
                {
                    Id = itemGuild.Id,
                    LobbyId = channel.Id
                };
                context.Add(lobbyData);
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteLobbyData(DiscordGuild guild)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == guild.Id);
            if (await query.AnyAsync() == true)
            {
                context.RoomersLobbies.Remove(query.First());
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<ItemRoomersLobby?> GetLobbyData(DiscordGuild guild)
        {
            using var context = _contextFactory.CreateDbContext();
            var query = context.RoomersLobbies.Where(x => x.Id == guild.Id);
            return await query.FirstOrDefaultAsync();
        }
        
    }
}
